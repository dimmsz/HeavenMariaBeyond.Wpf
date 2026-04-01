using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Enums;
using HeavenMaria.Domain.Interfaces;
using HeavenMaria.Service;
using HeavenMaria.Wpf.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static HeavenMaria.Service.IndexFinder;

namespace HeavenMaria.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IMainWindowQueryService _mainWindowQueryService;
        private readonly IMainWindowStatusService _mainWindowStatusService;
        private readonly IExternalLauncherService _externalLauncherService;
        private readonly IDiaryDisplayService _diaryDisplayService;
        private readonly ISelectModeStateService _selectModeStateService;
        private readonly IPathProvider _pathProvider;

        /// <summary>
        /// 嬢一覧 ＋ ステータス 
        /// </summary>
        public ObservableCollection<GirlWithStatusDto> Girls { get; set; } = new ObservableCollection<GirlWithStatusDto>();
        private GirlWithStatusDto _selectedGirl;
        public GirlWithStatusDto SelectedGirl
        {
            get => _selectedGirl;
            set
            {
                if (SetProperty(ref _selectedGirl, value) && value != null)
                {
                    UpdateDiaries(value.GirlId);
                }
            }
        }

        /// <summary>
        /// 日記コレクション
        /// </summary>
        public ObservableCollection<DiaryOld> Diaries { get; set; } = new();
        private DiaryOld _selectedDiary;
        public DiaryOld SelectedDiary
        {
            get => _selectedDiary;
            set => SetProperty(ref _selectedDiary, value, MoveToDiaryBodyViewAsync);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
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

            _selectModeStateService.SelectModeChanged += OnSelectModeChanged;

            //コマンド
            OnLoadedCommand = new DelegateCommand(OnLoaded);

            OpenDirectoryCommand = new DelegateCommand<string>(OpenDirectory);

            OpenSelectModeCommand = new DelegateCommand(OpenSelectMode);
            HideThisGirlCommand = new DelegateCommand(HideThisGirl);

            PlayVideoCommand = new DelegateCommand(PlayVideo);
            CallAppCommand = new DelegateCommand<string>(CallApp);
            OnlyMovieDiariesCommand = new DelegateCommand(OnlyMovieDiaries);
            OpenGirlProfileFolderCommand = new DelegateCommand(OpenGirlProfileFolder);
            OpenProfilePageCommand = new DelegateCommand(OpenProfilePage);
            OpenDiaryCommand = new DelegateCommand(OpenDiary);

            ReverseStatusCommand = new DelegateCommand<string>(ReverseStatus);

            SelectGirlsCommand = new DelegateCommand<string>(SelectGirls);

            DeleteDiaryCommand = new DelegateCommand(DeleteDiary);
        }

        /// <summary>
        /// 起動時
        /// </summary>
        public DelegateCommand OnLoadedCommand { get; }
        private void OnLoaded()
        {
            GirlsInitialize();
        }

        public void GirlsInitialize()
        {
            var notHiddenGirls = _mainWindowQueryService.GetVisibleGirlsSortedByName();
            Girls.Clear();
            Girls.AddRange(notHiddenGirls);
            SelectedGirl = Girls.FirstOrDefault();
        }

        /// <summary>
        /// フォルダを開く
        /// </summary>
        public DelegateCommand<string> OpenDirectoryCommand { get; }
        private void OpenDirectory(string param)
        {
            string dirPath = string.Empty;
            switch (param)
            {
                case "Diary":
                    dirPath = _pathProvider.GetDiaryDirectoryPath(SelectedDiary);
                    _externalLauncherService.OpenDirectory(dirPath);
                    break;
                case "Profile":
                    dirPath = _pathProvider.GetGirlProfileDirectoryPath(SelectedGirl.Girl);
                    _externalLauncherService.OpenDirectory(dirPath);
                    break;
                case "Old":
                    dirPath = _pathProvider.GetOldGirlDirectoryPath(SelectedGirl.Girl);
                    _externalLauncherService.OpenDirectory(dirPath);
                    break;
                case "Current":
                    dirPath = _pathProvider.GetDiaryDirectoryPath(SelectedDiary);
                    _externalLauncherService.OpenDirectory(dirPath);
                    break;
                default:
                    break;
            }
        }

        public DelegateCommand OpenDiaryCommand { get; }
        private void OpenDiary()
        {
            _externalLauncherService.OpenUrl(SelectedDiary?.DiaryUrl);
        }

        /// <summary>
        /// 嬢セレクトモードを開始する
        /// </summary>
        public DelegateCommand OpenSelectModeCommand { get; }
        private void OpenSelectMode()
        {
            if (!_selectModeStateService.IsInSelectMode)
            {
                _regionManager.RequestNavigate("ContentRegion", nameof(Views.SelectMode));
            }
        }

        /// <summary>
        /// この嬢を隠す
        /// </summary>
        public DelegateCommand HideThisGirlCommand { get; }
        private void HideThisGirl()
        {
            _mainWindowStatusService.Hide(SelectedGirl.GirlId);

            GirlsInitializeAndSelectGirl(SelectedGirl);
        }

        /// <summary>
        /// 動画を再生する
        /// </summary>
        public DelegateCommand PlayVideoCommand { get; }
        private void PlayVideo()
        {
            _externalLauncherService.PlayDiaryVideo(SelectedDiary);
        }

        /// <summary>
        /// 関連アプリを起動
        /// </summary>
        public DelegateCommand<string> CallAppCommand { get; }
        private void CallApp(string appName)
        {
            _externalLauncherService.LaunchRelatedApp(appName);
        }

        /// <summary>
        /// 動画日記のみ表示する
        /// </summary>
        public DelegateCommand OnlyMovieDiariesCommand { get; }
        private void OnlyMovieDiaries()
        {
            Diaries.Clear();
            var movieDiaries = _mainWindowQueryService.GetMovieDiaries(SelectedGirl.GirlId);
            //Diaries.AddRange(movieDiaries);
            SelectedDiary = Diaries.FirstOrDefault();
        }

        /// <summary>
        /// プロファイルフォルダを開く
        /// </summary>
        public DelegateCommand OpenGirlProfileFolderCommand { get; }
        private void OpenGirlProfileFolder()
        {
            _externalLauncherService.OpenDirectory(_selectedGirl.GirlId);
        }

        /// <summary>
        /// プロファイルをブラウザで開く
        /// </summary>
        public DelegateCommand OpenProfilePageCommand { get; }
        private void OpenProfilePage()
        {
            _externalLauncherService.OpenUrl(SelectedGirl.ProfileUrl);
        }

        /// <summary>
        /// 嬢コレクション初期化(インデックス指定)
        /// </summary>
        /// <param name="girl"></param>
        private void GirlsInitializeAndSelectGirl(GirlWithStatusDto girl)
        {
            var nextGirlIndex = GetGirlIndex(Girls.ToList(), SelectedGirl);

            var girls = _mainWindowQueryService.GetVisibleGirlsSortedByName().ToList();

            Girls.Clear();
            Girls.AddRange(girls);
            SelectedGirl = Girls[nextGirlIndex];
        }

        /// <summary>
        /// ステータスを反転させる
        /// </summary>
        public DelegateCommand<string> ReverseStatusCommand { get; }
        private void ReverseStatus(string param)
        {
            var attribute = Enum.Parse<GirlAttribute>(param);
            var result = _mainWindowStatusService.ReverseStatus(
                SelectedGirl.GirlId,
                attribute,
                SelectedGirl.Favorite,
                SelectedGirl.Legend);

            if (!result.Applied)
            {
                return;
            }

            if (result.VisibilityChanged)
            {
                GirlsInitializeAndSelectGirl(SelectedGirl);
            }

            var newSelectedGirl = _mainWindowQueryService.GetGirlById(SelectedGirl.GirlId);
            SelectedGirl = newSelectedGirl;
        }

        /// <summary>
        /// 嬢の選択
        /// </summary>
        public DelegateCommand<string> SelectGirlsCommand { get; }
        private void SelectGirls(string keyString)
        {
            var key = Enum.Parse<GirlKeyType>(keyString);

            var girls = _mainWindowQueryService.GetGirlsByKey(key);

            Girls.Clear();
            Girls.AddRange(girls);
            SelectedGirl = girls.FirstOrDefault();
        }

        /// <summary>
        /// ブログを表示ビューへ移動
        /// </summary>
        private async void MoveToDiaryBodyViewAsync()
        {
            if (SelectedDiary == null)
            {
                return;
            }

            var html = await _diaryDisplayService.BuildDiaryHtml(_selectedDiary.DiaryCode);

            var param = new NavigationParameters
            {
                { "BlogBodyHtml", html }
            };

            _regionManager.RequestNavigate("ContentRegion", nameof(BlogDisplay), param);
        }

        private void UpdateDiaries(string girlId)
        {
            if (string.IsNullOrEmpty(girlId) || _selectModeStateService.IsInSelectMode) return;

            Diaries.Clear();
            var diaries = _mainWindowQueryService.GetDiariesByGirlIdDescending(girlId);
            Diaries.AddRange(diaries);
            SelectedDiary = Diaries.FirstOrDefault();
        }

        private void OnSelectModeChanged(bool isInSelectMode)
        {
            if (!isInSelectMode)
            {
                GirlsInitialize();
            }
        }

        public DelegateCommand DeleteDiaryCommand { get; }
        private void DeleteDiary()
        {
            if (SelectedDiary == null)
            {
                return;
            }
            var currentIndex = Diaries.IndexOf(SelectedDiary);
            _mainWindowStatusService.DeleteDiary(SelectedDiary);
            UpdateDiaries(SelectedGirl.GirlId);
            SelectedDiary = Diaries.ElementAtOrDefault(currentIndex) ?? Diaries.LastOrDefault();
        }
    }
}
