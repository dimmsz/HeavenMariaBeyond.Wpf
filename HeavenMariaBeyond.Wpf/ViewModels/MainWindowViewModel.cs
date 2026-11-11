using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;
using HeavenMariaBeyond.Domain.Interfaces;
using HeavenMariaBeyond.Service;
using HeavenMariaBeyond.Wpf.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeavenMariaBeyond.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        internal const string ContentRegion = "ContentRegion";

        private readonly IRegionManager _regionManager;
        private readonly IMainWindowQueryService _mainWindowQueryService;
        private readonly IMainWindowStatusService _mainWindowStatusService;
        private readonly IExternalLauncherService _externalLauncherService;
        private readonly IDiaryDisplayService _diaryDisplayService;
        private readonly ISelectModeStateService _selectModeStateService;
        private readonly IPathProvider _pathProvider;
        private CancellationTokenSource _diaryRequest;
        private bool _loaded;

        public bool CanBrowse => _loaded && !_selectModeStateService.IsInSelectMode;

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public ObservableCollection<GirlWithStatusDto> Girls { get; } = new();
        private GirlWithStatusDto _selectedGirl;
        public GirlWithStatusDto SelectedGirl
        {
            get => _selectedGirl;
            set
            {
                if (SetProperty(ref _selectedGirl, value) && CanBrowse)
                {
                    UpdateDiaries(value);
                }
            }
        }

        public ObservableCollection<Diary> Diaries { get; } = new();
        private Diary _selectedDiary;
        public Diary SelectedDiary
        {
            get => _selectedDiary;
            set
            {
                if (SetProperty(ref _selectedDiary, value))
                {
                    _ = DisplayDiaryAsync(value);
                }
            }
        }

        public MainWindowViewModel(IRegionManager regionManager,
            IMainWindowQueryService mainWindowQueryService,
            IMainWindowStatusService mainWindowStatusService,
            IExternalLauncherService externalLauncherService,
            IDiaryDisplayService diaryDisplayService,
            ISelectModeStateService selectModeStateService,
            IPathProvider pathProvider)
        {
            _regionManager = regionManager;
            _mainWindowQueryService = mainWindowQueryService;
            _mainWindowStatusService = mainWindowStatusService;
            _externalLauncherService = externalLauncherService;
            _diaryDisplayService = diaryDisplayService;
            _selectModeStateService = selectModeStateService;
            _pathProvider = pathProvider;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OnClosedCommand = new DelegateCommand(OnClosed);
            OpenSelectModeCommand = new DelegateCommand(OpenSelectMode, () => CanBrowse).ObservesProperty(() => CanBrowse);
            SelectGirlsCommand = new DelegateCommand<GirlKeyType?>(key =>
            {
                if (key is { } filter)
                    LoadGirls(filter);
            });

            OpenDirectoryCommand = new DelegateCommand<string>(OpenDirectory, CanOpenDirectory)
                .ObservesProperty(() => SelectedGirl).ObservesProperty(() => SelectedDiary);
            OpenProfilePageCommand = new DelegateCommand(
                () => Run("プロフィールを開けませんでした。", () => _externalLauncherService.OpenUrl(SelectedGirl.ProfileUrl)),
                () => !string.IsNullOrWhiteSpace(SelectedGirl?.ProfileUrl)).ObservesProperty(() => SelectedGirl);
            OpenDiaryCommand = new DelegateCommand(
                () => Run("日記ページを開けませんでした。", () => _externalLauncherService.OpenUrl(SelectedDiary.DiaryUrl)),
                () => !string.IsNullOrWhiteSpace(SelectedDiary?.DiaryUrl)).ObservesProperty(() => SelectedDiary);
            PlayVideoCommand = new DelegateCommand(
                () => Run("動画を再生できませんでした。", () => _externalLauncherService.PlayDiaryVideo(SelectedDiary)),
                () => SelectedDiary != null).ObservesProperty(() => SelectedDiary);
            OnlyMovieDiariesCommand = new DelegateCommand(OnlyMovieDiaries, () => SelectedGirl != null)
                .ObservesProperty(() => SelectedGirl);
            HideThisGirlCommand = new DelegateCommand(() => ReverseStatus(GirlAttribute.Hidden), () => SelectedGirl != null)
                .ObservesProperty(() => SelectedGirl);
            ReverseStatusCommand = new DelegateCommand<GirlAttribute?>(ReverseStatus, _ => SelectedGirl != null)
                .ObservesProperty(() => SelectedGirl);
            DeleteDiaryCommand = new DelegateCommand(DeleteDiary, () => SelectedDiary != null)
                .ObservesProperty(() => SelectedDiary);
        }

        public DelegateCommand OnLoadedCommand { get; }
        public DelegateCommand OnClosedCommand { get; }
        public DelegateCommand OpenSelectModeCommand { get; }
        public DelegateCommand<GirlKeyType?> SelectGirlsCommand { get; }
        public DelegateCommand<string> OpenDirectoryCommand { get; }
        public DelegateCommand OpenProfilePageCommand { get; }
        public DelegateCommand OpenDiaryCommand { get; }
        public DelegateCommand PlayVideoCommand { get; }
        public DelegateCommand OnlyMovieDiariesCommand { get; }
        public DelegateCommand HideThisGirlCommand { get; }
        public DelegateCommand<GirlAttribute?> ReverseStatusCommand { get; }
        public DelegateCommand DeleteDiaryCommand { get; }

        private void OnLoaded()
        {
            if (_loaded)
                return;

            _loaded = true;
            _selectModeStateService.SelectModeChanged += OnSelectModeChanged;
            RaisePropertyChanged(nameof(CanBrowse));
            LoadGirls(GirlKeyType.NotHidden);
        }

        private void OnClosed()
        {
            _loaded = false;
            _selectModeStateService.SelectModeChanged -= OnSelectModeChanged;
            _diaryRequest?.Cancel();
            _selectModeStateService.ExitSelectMode();
        }

        private void LoadGirls(GirlKeyType filter, int selectionIndex = 0)
        {
            Run("一覧を取得できませんでした。", () =>
            {
                var girls = _mainWindowQueryService.GetGirlsByKey(filter).ToList();
                Girls.Clear();
                Girls.AddRange(girls);
                SelectedGirl = girls.ElementAtOrDefault(selectionIndex) ?? girls.LastOrDefault();
            });
        }

        private void UpdateDiaries(GirlWithStatusDto girl)
        {
            if (girl == null)
            {
                SetDiaries(Array.Empty<Diary>());
                return;
            }

            if (!Run("日記一覧を取得できませんでした。", () =>
                SetDiaries(_mainWindowQueryService.GetDiaryPartsByGirlIdDescending(girl.GirlId).ToList())))
            {
                SetDiaries(Array.Empty<Diary>());
            }
        }

        private void OnlyMovieDiaries()
        {
            if (SelectedGirl is { } girl)
                Run("動画日記を取得できませんでした。", () =>
                    SetDiaries(_mainWindowQueryService.GetMovieDiaries(girl.GirlId).ToList()));
        }

        private void SetDiaries(IReadOnlyList<Diary> diaries)
        {
            Diaries.Clear();
            Diaries.AddRange(diaries);
            SelectedDiary = diaries.FirstOrDefault();
        }

        private bool CanOpenDirectory(string target)
            => target switch
            {
                "Diary" or "Current" => SelectedDiary != null,
                "Profile" or "Old" => SelectedGirl != null,
                _ => false
            };

        private void OpenDirectory(string target)
        {
            if (!CanOpenDirectory(target))
                return;

            Run("フォルダーを開けませんでした。", () =>
            {
                switch (target)
                {
                    case "Diary":
                    case "Current":
                        _externalLauncherService.OpenDirectory(SelectedDiary);
                        break;
                    case "Profile":
                        _externalLauncherService.OpenDirectory(SelectedGirl.Girl);
                        break;
                    case "Old":
                        _externalLauncherService.OpenDirectory(_pathProvider.GetOldGirlDirectoryPath(SelectedGirl.Girl));
                        break;
                }
            });
        }

        private void ReverseStatus(GirlAttribute? attribute)
        {
            if (SelectedGirl is not { } girl || attribute is not { } flag)
                return;

            var index = Girls.IndexOf(girl);
            Run("状態を更新できませんでした。", () =>
            {
                var result = _mainWindowStatusService.ReverseStatus(girl.GirlId, flag, girl.Favorite, girl.Legend);
                if (!result.Applied)
                    return;

                if (result.VisibilityChanged)
                {
                    LoadGirls(GirlKeyType.NotHidden, index);
                }
                else if (index >= 0)
                {
                    var updated = ToggleGirlAttributeService.Toggle(girl, flag);
                    Girls[index] = updated;
                    SelectedGirl = updated;
                }
            });
        }

        private void DeleteDiary()
        {
            if (SelectedDiary is not { } diary)
                return;

            Run("日記を削除できませんでした。", () =>
            {
                var index = Diaries.IndexOf(diary);
                _mainWindowStatusService.DeleteDiary(diary);
                Diaries.Remove(diary);
                SelectedDiary = Diaries.ElementAtOrDefault(index) ?? Diaries.LastOrDefault();
            });
        }

        private void OpenSelectMode()
        {
            if (CanBrowse)
                Run("セレクトモードを開けませんでした。", () =>
                    Navigate(nameof(SelectMode), new NavigationParameters(), "セレクトモードを開けませんでした。"));
        }

        private void OnSelectModeChanged(bool isInSelectMode)
        {
            RaisePropertyChanged(nameof(CanBrowse));
            if (isInSelectMode)
                _diaryRequest?.Cancel();
            else
                LoadGirls(GirlKeyType.NotHidden);
        }

        private async Task DisplayDiaryAsync(Diary diary)
        {
            _diaryRequest?.Cancel();
            if (!CanBrowse)
                return;

            using var request = new CancellationTokenSource();
            _diaryRequest = request;
            try
            {
                // Clear the previous diary while a new one is loading.
                if (_regionManager.Regions.ContainsRegionWithName(ContentRegion) &&
                    _regionManager.Regions[ContentRegion].ActiveViews.OfType<BlogDisplay>().Any())
                {
                    NavigateToDiary(string.Empty);
                }

                if (diary == null)
                    return;

                ErrorMessage = string.Empty;
                var html = await _diaryDisplayService.BuildDiaryHtml(diary.DiaryCode, request.Token);
                if (!request.IsCancellationRequested)
                    NavigateToDiary(html);
            }
            catch (OperationCanceledException) when (request.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
                if (!request.IsCancellationRequested)
                    ErrorMessage = "日記本文を表示できませんでした。";
            }
            finally
            {
                if (_diaryRequest == request)
                    _diaryRequest = null;
            }
        }

        private void NavigateToDiary(string html)
        {
            var parameters = new NavigationParameters { { BlogDisplayViewModel.HtmlParameter, html } };
            Navigate(nameof(BlogDisplay), parameters, "日記本文を表示できませんでした。");
        }

        private void Navigate(string view, NavigationParameters parameters, string failureMessage)
        {
            _regionManager.RequestNavigate(ContentRegion, view, result =>
            {
                if (!result.Success)
                {
                    if (result.Exception is { } exception)
                        Trace.TraceError(exception.ToString());
                    ErrorMessage = failureMessage;
                }
            }, parameters);
        }

        private bool Run(string failureMessage, Action action)
        {
            ErrorMessage = string.Empty;
            try
            {
                action();
                return true;
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
                ErrorMessage = failureMessage;
                return false;
            }
        }
    }
}
