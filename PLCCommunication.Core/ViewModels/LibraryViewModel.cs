using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Windows.Data;
using System.Windows.Threading;
using WPFCommon.ViewModels.Base;

namespace PLCCommunication.Core.ViewModels
{
    public class LibraryViewModel  : ViewModelBase
    {

        public static LibraryViewModel Instance => _instance;
        private static LibraryViewModel _instance = new LibraryViewModel();
        
        private AlcServerViewModel _serverViewModel;

        public AlcServerViewModel ServerViewModel => _serverViewModel;

        public ObservableCollection<string> MessageList { get; set; } = new ObservableCollection<string>();
        private object _lockerOfMessageList = new object();

        public LibraryViewModel()
        {
            _serverViewModel = new AlcServerViewModel()
            {
                IpAddress = IPAddress.Parse("192.168.100.100"),
                Port = 4000,
            };
            _serverViewModel.ClientHooked += OnClientHooked;
            _serverViewModel.PlcResetRequested += OnPlcResetRequested;
            _serverViewModel.PlcRegisterRequested += OnRegistered;
            _serverViewModel.PlcRequestToEnterNewRun += OnPlcRequestToEnterNewRequested;
            _serverViewModel.PlcStopRequested += OnAutoStopRequested;
            
            BindingOperations.EnableCollectionSynchronization(MessageList, _lockerOfMessageList);
        }

        private void OnAutoStopRequested()
        {
            LogThreadSafe("Auto-stop requested");
        }

        private void OnPlcRequestToEnterNewRequested()
        {
            LogThreadSafe("Auto-start requested");
        }

        private void OnRegistered()
        {
            LogThreadSafe("Plc registered");
        }

        private void OnPlcResetRequested()
        {
            LogThreadSafe("Init requested");
        }

        private void OnClientHooked(Socket obj)
        {
            LogThreadSafe(obj.RemoteEndPoint + " connected");
        }

        private void LogThreadSafe(string message)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                lock (_lockerOfMessageList)
                {
                    MessageList.Add(message);
                }
            });
        }

      


    }
}