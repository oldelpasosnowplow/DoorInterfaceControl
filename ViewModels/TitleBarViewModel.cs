using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System.Windows;
using DoorInterfaceControl.Events;


namespace DoorInterfaceControl.ViewModels
{
    public class TitleBarViewModel : BindableBase
    {
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand MinimizeCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }

        readonly IEventAggregator _eventAggregator;
        public TitleBarViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            RefreshCommand = new DelegateCommand(RefreshExecute);
            MinimizeCommand = new DelegateCommand(MinimizeExecute);
            ExitCommand = new DelegateCommand(ExitExecute);
        }

        public void RefreshExecute()
        {
            RefreshScreen(true);
        }

        public void RefreshScreen(bool isRefresh)
        {
            // Publish to the event aggregator so this can call the clearscreen (refresh) function on DoorManagementViewModel
            _eventAggregator.GetEvent<RefreshEvents>().Publish(isRefresh);
        }

        private void MinimizeExecute()
        {
            Application.Current.MainWindow.Hide();
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void ExitExecute()
        {
            Application.Current.MainWindow.Close();
        }
    }
}
