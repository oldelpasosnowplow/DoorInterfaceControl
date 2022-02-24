using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using System.Security.Principal;
using System.DirectoryServices;
using System.Configuration;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using System;

namespace DoorInterfaceControl.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Door Interface Control";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _appIcon = "/DoorInterfaceControl;component/Resources/Door.ico";
        public string AppIcon
        {
            get { return _appIcon; }
            set { SetProperty(ref _appIcon, value); }
        }

        public static DelegateCommand<object> ShowCommand { get; set; }
        public static DelegateCommand<object> MinimizeCommand { get; set; }

        private readonly IRegionManager _regionManager;
        public MainWindowViewModel(IRegionManager regionManager)
        {
            ShowCommand = new DelegateCommand<object>(ShowForm);
            MinimizeCommand = new DelegateCommand<object>(MinimizeForm);

            if (CheckUserSecurity())
            {
                WriteLog("Past Security Check").GetAwaiter().GetResult();
                _regionManager = regionManager.RegisterViewWithRegion("ContentRegion", typeof(Views.DoorManagement)).RegisterViewWithRegion("TitleRegion", typeof(Views.TitleBar));
            }
            else
            {
                Application.Current.MainWindow.Close();
            }
        }

        public void ShowForm(object parameter)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        public void MinimizeForm(object parameter)
        {
            Application.Current.MainWindow.Hide();
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private bool CheckUserSecurity()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            string username = GetUserName(principal.Identity.Name.ToString());

            using (DirectorySearcher adSearcher = new(GetDirEntry()))
            {
                adSearcher.Filter = "(&(objectCategory=person)(SAMAccountName=" + username + "))";
                adSearcher.SearchScope = SearchScope.Subtree;
                adSearcher.PropertiesToLoad.Add("memberOf");
                SearchResult searchResult = adSearcher.FindOne();
                
                if (searchResult != null)
                {
                    int mCount = searchResult.Properties["memberOf"].Count;

                    for (int count = 1; count < mCount; count++)
                    {
                        string fullGroupName = (string)searchResult.Properties["memberOf"][count];
                        string groupName = fullGroupName.Substring(fullGroupName.IndexOf("=") + 1, fullGroupName.IndexOf(",") - 3);

                        WriteLog(groupName).GetAwaiter().GetResult();

                        if (groupName is "LEX_Information Systems" or "LEX_Human Resources")
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private string GetUserName(string uname)
        {
            string[] uwords = uname.Split('\\');

            string cleanName = uwords[1];
            WriteLog(cleanName).GetAwaiter().GetResult();
            return cleanName;
        }

        private static DirectoryEntry GetDirEntry()
        {
            DirectoryEntry dirEntry = new();
            dirEntry.Path = ConfigurationManager.AppSettings["LDAPPath"];
            dirEntry.Username = ConfigurationManager.AppSettings["LDAPUsername"];
            dirEntry.Password = ConfigurationManager.AppSettings["LDAPPassword"];
            dirEntry.AuthenticationType = AuthenticationTypes.Secure;

            return dirEntry;
        }

        private static async Task WriteLog(string message)
        {
            using StreamWriter file = new("DoorLog.txt", append: true);
            await file.WriteLineAsync(message + Environment.NewLine);
        }
    }
}
