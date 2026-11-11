using HeavenMariaBeyond.Service;
using HeavenMariaBeyond.Wpf.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using SelectMode = HeavenMariaBeyond.Wpf.Views.SelectMode;

namespace HeavenMariaBeyond.Wpf
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
            containerRegistry.RegisterHeavenMariaBeyondCore();

            // SelectMode を登録
            containerRegistry.RegisterForNavigation<SelectMode>();

            // BlogDisplay を登録
            containerRegistry.RegisterForNavigation<BlogDisplay>();
        }
    }
}
