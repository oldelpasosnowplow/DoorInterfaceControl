using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Configuration;
using System.Xml.Linq;
using System.Linq;
using System.Collections.ObjectModel;
using DoorInterfaceControl.Class;
using DoorInterfaceControl.Events;


namespace DoorInterfaceControl.ViewModels
{
    public class DoorManagementViewModel : BindableBase
    {
        private static HttpClient client = new();

        private ObservableCollection<AccessPoints> _doorList = new AsyncObservableCollection<AccessPoints>();
        public ObservableCollection<AccessPoints> DoorList
        {
            get { return _doorList; }
            set { SetProperty(ref _doorList, value); }
        }

        private ObservableCollection<AccessPoints> _selectedDoorList = new();
        public ObservableCollection<AccessPoints> SelectedDoorList
        {
            get { return _selectedDoorList; }
            set { SetProperty(ref _selectedDoorList, value); }
        }

        private ObservableCollection<string> _stateList = new();
        public ObservableCollection<string> StateList
        {
            get { return _stateList; }
            set { SetProperty(ref _stateList, value); }
        }

        private string _stateName = string.Empty;
        public string StateName
        {
            get { return _stateName; }
            set { SetProperty(ref _stateName, value); }
        }

        private string _stateChanged = string.Empty;
        public string StateChanged
        {
            get { return _stateChanged; }
            set { SetProperty(ref _stateChanged, value); }
        }

        private string _allStateChanged = string.Empty;
        public string AllStateChanged
        {
            get { return _allStateChanged; }
            set { SetProperty(ref _allStateChanged, value); }
        }

        private List<string> _checkedDoor = new();
        public List<string> CheckedDoor
        {
            get { return _checkedDoor; }
            set { SetProperty(ref _checkedDoor, value); }
        }

        public DelegateCommand ExecuteCommand { get; set; }
        public DelegateCommand<string> AllDoorCommand { get; set; }
        public DelegateCommand<AccessPoints> SelectCommand { get; set; }

        readonly IEventAggregator _eventAggregator;
        public DoorManagementViewModel( IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<RefreshEvents>().Subscribe(ClearScreen);
            ExecuteCommand = new DelegateCommand(ExecuteFunction);
            SelectCommand = new DelegateCommand<AccessPoints>(SelectExecute);
            AllDoorCommand = new DelegateCommand<string>(AllDoorExecute);
            
            RunAsync();
            GetDoorList().GetAwaiter().GetResult();
            ReadXML();
        }

        private void ExecuteFunction()
        {
            if ((StateName != string.Empty) || (CheckedDoor.Count != 0))
            {
                APCommands c = new();
                c.command = StateName;
                HttpResponseMessage response = new();
                foreach (string cd in CheckedDoor)
                {
                    c.id = cd;
                    response = client.PostAsJsonAsync($"api/v1/accesspoint/{c.id}/state/{c.command}", c).Result;
                    GetDoorList().GetAwaiter().GetResult();

                    foreach (AccessPoints ap in SelectedDoorList)
                    {
                        SelectExecute(ap);
                        if (SelectedDoorList.Count == 0)
                            break;
                    }

                    if (CheckedDoor.Count == 0)
                        break;
                }

                if (response.IsSuccessStatusCode)
                {
                    StateChanged = "Changed to " + StateName;
                    ClearScreen(true);
                }
            }
        }

        private void ClearScreen(bool isRefresh)
        {
            if (isRefresh)
            {
                DoorList.Clear();
                CheckedDoor.Clear();
                SelectedDoorList.Clear();
                GetDoorList().GetAwaiter().GetResult();
                ReadXML();
            }
        }

        private void SelectExecute(AccessPoints ap)
        {
            if (CheckedDoor.Contains(ap.ID.ToString()))
            {
                CheckedDoor.Remove(ap.ID.ToString());
                SelectedDoorList.Remove(ap);
            }
            else
            {
                CheckedDoor.Add(ap.ID.ToString());
                SelectedDoorList.Add(ap);
            }
        }

        private void AllDoorExecute(string parameter)
        {
            APCommands c = new();
            c.command = parameter;
            HttpResponseMessage response = new();
            foreach (AccessPoints ap in DoorList)
            {
                c.id = ap.ID.ToString();
                response = client.PostAsJsonAsync($"api/v1/accesspoint/{c.id}/state/{c.command}", c).Result;
            }
            if (response.IsSuccessStatusCode)
            {
                if (parameter == "schedule")
                    parameter = "normal";
                AllStateChanged = "All Doors " + parameter;
            }
            GetDoorList().GetAwaiter().GetResult();
        }

        private static void RunAsync()
        {
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["APIWebURL"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string userPassword = ConfigurationManager.AppSettings["APIUsername"].ToString() + ":" + ConfigurationManager.AppSettings["APIPassword"].ToString();
            string authorization = Convert.ToBase64String(Encoding.Default.GetBytes(userPassword));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorization);
        }

        private async Task GetDoorList()
        {
            HttpResponseMessage response = await client.GetAsync("api/v1/accesspoints/").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                AccessPoints[] dlist = await response.Content.ReadAsAsync<AccessPoints[]>();
                ObservableCollection <AccessPoints> tempDoorList = new();

                foreach (AccessPoints ap in dlist)
                {
                    HttpResponseMessage doorResponse = await client.GetAsync($"api/v1/accesspoint/{ap.ID}/state").ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        AccessPointState aps = await doorResponse.Content.ReadAsAsync<AccessPointState>();
                        ap.state = aps.state;
                        ap.lockStatus = aps.lockStatus;
                    }
                    tempDoorList.Add(ap);
                }
                DoorList = tempDoorList;
            }
        }
        
        private void ReadXML()
        {
            StateList.Clear();
            XDocument xml = XDocument.Load(ConfigurationManager.AppSettings["DoorStateXML"].ToString());
            IEnumerable<XElement> query = from x in xml.Root.Descendants("State") select x;

            string sl = string.Empty;
            ObservableCollection<string> tempStateList = new();
            foreach (XElement node in query)
            {
                if (node.HasElements)
                {
                   sl = node.Element("name").Value.ToString();
                   tempStateList.Add(sl);
                }
            }
            StateList = tempStateList;
        }
    }
}
