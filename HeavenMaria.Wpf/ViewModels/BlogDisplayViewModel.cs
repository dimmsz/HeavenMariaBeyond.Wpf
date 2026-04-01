using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace HeavenMaria.Wpf.ViewModels
{
    public class BlogDisplayViewModel : BindableBase, INavigationAware
    {
        //************************************************
        private readonly IRegionManager _regionManager;
        //************************************************

        /// <summary>
        /// WebBrowserのDocumentText
        /// </summary>
        private string _html;
        public string Html
        {
            get => _html;
            set => SetProperty(ref _html, value);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BlogDisplayViewModel(IRegionManager regionManager)
        {
            //リージョンマネージャー
            _regionManager = regionManager;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <summary>
        /// 遷移直前
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// ここから
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Html = navigationContext.Parameters.GetValue<string>("BlogBodyHtml");
        }
    }
}
