using HeavenMaria.Service;
using HeavenMaria.Wpf.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using SelectMode = HeavenMaria.Wpf.Views.SelectMode;

namespace HeavenMaria.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterHeavenMariaCore();

            //セレクトモード
            containerRegistry.RegisterForNavigation<SelectMode>();

            //ブログ表示ブラウザ
            containerRegistry.RegisterForNavigation<BlogDisplay>();
        }
    }
}
