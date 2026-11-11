using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Service;
using HeavenMariaBeyond.Wpf.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HeavenMariaBeyond.Wpf.ViewModels
{
    public class SelectModeViewModel : BindableBase, INavigationAware
    {
        private readonly ISelectModeService _selectModeService;
        private readonly IRegionManager _regionManager;
        private readonly ISelectModeStateService _selectModeStateService;

        private IReadOnlyList<GirlSelection> _girls = Array.Empty<GirlSelection>();
        public IReadOnlyList<GirlSelection> Girls
        {
            get => _girls;
            private set => SetProperty(ref _girls, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public SelectModeViewModel(IRegionManager regionManager,
            ISelectModeService selectModeService,
            ISelectModeStateService selectModeStateService)
        {
            _regionManager = regionManager;
            _selectModeService = selectModeService;
            _selectModeStateService = selectModeStateService;

            SelectAllGirlsCommand = new DelegateCommand(() => SetAllHidden(true));
            DeSelectAllGirlsCommand = new DelegateCommand(() => SetAllHidden(false));
            ExitSelectModeCommand = new DelegateCommand(ExitSelectMode);
        }

        public DelegateCommand SelectAllGirlsCommand { get; }
        public DelegateCommand DeSelectAllGirlsCommand { get; }
        public DelegateCommand ExitSelectModeCommand { get; }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _selectModeStateService.EnterSelectMode();
            Girls = Array.Empty<GirlSelection>();
            Run("セレクトモードの一覧を取得できませんでした。", () =>
            {
                Girls = _selectModeService.GetAllGirlsSortedByName()
                    .Select(girl => new GirlSelection(this, girl)).ToArray();
            });
        }

        private void ExitSelectMode()
        {
            const string failureMessage = "セレクトモードを終了できませんでした。";
            Run(failureMessage, () =>
                _regionManager.RequestNavigate(MainWindowViewModel.ContentRegion, nameof(BlogDisplay), result =>
                {
                    if (result.Success)
                    {
                        _selectModeStateService.ExitSelectMode();
                    }
                    else
                    {
                        if (result.Exception is { } exception)
                            Trace.TraceError(exception.ToString());
                        ErrorMessage = failureMessage;
                    }
                }));
        }

        private void SetAllHidden(bool hidden)
        {
            var changed = Girls.Where(girl => girl.Hidden != hidden).ToArray();
            SaveHidden(changed, hidden);
        }

        private bool SaveHidden(IReadOnlyCollection<GirlSelection> girls, bool hidden)
        {
            if (girls.Count == 0)
                return true;

            return Run("表示設定を保存できませんでした。", () =>
            {
                _selectModeService.SetHidden(girls.Select(girl => girl.GirlId).ToArray(), hidden);
                foreach (var girl in girls)
                    girl.ApplyHidden(hidden);
            });
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

        public sealed class GirlSelection : BindableBase
        {
            private readonly SelectModeViewModel _owner;
            private bool _hidden;

            public string GirlId { get; }
            public string DisplayName { get; }
            public bool Hidden
            {
                get => _hidden;
                set
                {
                    if (_hidden != value && !_owner.SaveHidden(new[] { this }, value))
                        RaisePropertyChanged(nameof(Hidden));
                }
            }

            internal GirlSelection(SelectModeViewModel owner, GirlWithStatusDto girl)
            {
                _owner = owner;
                GirlId = girl.GirlId;
                DisplayName = girl.DisplayName;
                _hidden = girl.Hidden;
            }

            internal void ApplyHidden(bool hidden) => SetProperty(ref _hidden, hidden, nameof(Hidden));
        }
    }
}
