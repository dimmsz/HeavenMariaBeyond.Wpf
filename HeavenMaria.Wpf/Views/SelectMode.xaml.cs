using HeavenMaria.Domain.Entities.Dtos;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Wpf.ViewModels;
using System.Windows.Controls;

namespace HeavenMaria.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SelectMode
    /// </summary>
    public partial class SelectMode : UserControl
    {
        public SelectMode()
        {
            InitializeComponent();
        }

        private void GirlSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (DataContext is SelectModeViewModel vm)
            {
                vm.GirlSelectionChangedCommand.Execute(e);
            }
        }
    }
}
