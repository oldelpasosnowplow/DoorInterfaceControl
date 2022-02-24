using DoorInterfaceControl.Views;
using Prism.Ioc;
using System.Windows;
using Prism.Unity;

namespace DoorInterfaceControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
