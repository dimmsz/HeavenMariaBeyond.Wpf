using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace HeavenMariaBeyond.Wpf.ViewModels
{
    public class BlogDisplayViewModel : BindableBase, INavigationAware
    {
        internal const string HtmlParameter = "BlogBodyHtml";

        /// <summary>
        /// WebBrowserのDocumentText
        /// </summary>
        private string _html = string.Empty;
        public string Html
        {
            get => _html;
            private set => SetProperty(ref _html, value);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        /// <summary>
        /// 遷移直前
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        /// <summary>
        /// ここから
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Html = navigationContext.Parameters.GetValue<string>(HtmlParameter) ?? string.Empty;
        }
    }
}
