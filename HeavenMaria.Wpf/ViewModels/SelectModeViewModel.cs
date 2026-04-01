using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Service;
using HeavenMaria.Wpf.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xceed.Wpf.Toolkit.Primitives;

namespace HeavenMaria.Wpf.ViewModels
{
    public class SelectModeViewModel : BindableBase, INavigationAware
    {
        private bool _listViewInitialized = false;

        private readonly ISelectModeService _selectModeService;

        //************************************************
        private readonly IRegionManager _regionManager;
        //************************************************

        private readonly ISelectModeStateService _selectModeStateService;

        private List<GirlWithStatusDto> BeforeSelectedGirls = new List<GirlWithStatusDto>();

        /// <summary>
        /// 嬢コレクション
        /// </summary>
        public ObservableCollection<GirlWithStatusDto> Girls { get; set; } = new ObservableCollection<GirlWithStatusDto>();

        /// <summary>
        /// 選択された嬢コレクション
        /// </summary>
        public ObservableCollection<GirlWithStatusDto> SelectedGirls { get; set; } = new ObservableCollection<GirlWithStatusDto>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SelectModeViewModel(IRegionManager regionManager,
            ISelectModeService selectModeService,
            ISelectModeStateService selectModeStateService)
        {
            _regionManager = regionManager;
            _selectModeService = selectModeService;
            _selectModeStateService = selectModeStateService;

            SelectAllGirlsCommand = new DelegateCommand(SelectAllGirls);
            DeSelectAllGirlsCommand = new DelegateCommand(DeSelectAllGirls);
            ExitSelectModeCommand = new DelegateCommand(ExitSelectMode);
            GirlSelectionChangedCommand = new DelegateCommand<object>(OnGirlChanged);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <summary>
        /// 遷移直前
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        /// <summary>
        /// 遷移後ここが呼ばれる
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _selectModeStateService.EnterSelectMode();

            var girls = _selectModeService.GetAllGirlsSortedByName().ToList();

            Girls.Clear();
            Girls.AddRange(girls);

            ///このCheckListBoxにおいて、
            ///IsCheckedにtrueが設定されるのは、
            ///ItemsSourceとSelectedItemsOverrideが一致した場合である。
            ///値型のintなら問題ないが、
            ///ItemsSourceに参照型の任意のクラスのコレクションを設定した場合、
            ///当該任意のクラスのインスタンスが一致するのは
            ///参照アドレスが一致したときである。
            ///よって、ItemsSourceに設定したインスタンスのコレクションと
            ///同じコレクションをSelectedItemsOverrideで設定しなければならない。

            SelectedGirls.AddRange(girls.Where(x => x.Hidden));
            BeforeSelectedGirls.AddRange(SelectedGirls);
            _listViewInitialized = true;
        }

        /// <summary>
        /// すべて選択
        /// </summary>
        public DelegateCommand SelectAllGirlsCommand { get; }
        private void SelectAllGirls()
        {
            SelectedGirls.Clear();
            SelectedGirls.AddRange(Girls);
        }

        /// <summary>
        /// すべて選択解除
        /// </summary>
        public DelegateCommand DeSelectAllGirlsCommand { get; }
        private void DeSelectAllGirls()
        {
            SelectedGirls.Clear();
        }

        /// <summary>
        /// セレクトモードから抜ける
        /// </summary>
        public DelegateCommand ExitSelectModeCommand { get; }
        private void ExitSelectMode()
        {
            //遷移
            _regionManager.RequestNavigate("ContentRegion", nameof(BlogDisplay));

            //セレクトモードのフラグを下げる
            _selectModeStateService.ExitSelectMode();

            _listViewInitialized = false;
        }

        public DelegateCommand<object> GirlSelectionChangedCommand { get; }
        private void OnGirlChanged(object param)
        {
            if (!_listViewInitialized)
            {
                return;
            }

            var arg = param as ItemSelectionChangedEventArgs;
            var dto = arg.Item as GirlWithStatusDto;

            var girl = Girls
                .Where(x => x.GirlId == dto.GirlId)
                .First();

            if (girl.Hidden)
            {
                _selectModeService.UpdateHiddenByCurrentState(dto.GirlId, currentlyHidden: true);
            }
            else
            {
                _selectModeService.UpdateHiddenByCurrentState(dto.GirlId, currentlyHidden: false);
            }
        }
    }
}
