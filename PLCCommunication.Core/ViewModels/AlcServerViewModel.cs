using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PLCCommunication.Core.Enums;
using PLCCommunication.Core.Interfaces;
using WPFCommon.Commands;
using WPFCommon.Helpers;
using WPFCommon.ViewModels.Base;

namespace PLCCommunication.Core.ViewModels
{
    public sealed class AlcServerViewModel
    {
        private Socket _serverSocket;

        private Socket _clientSocket;

        private DateTime _timePreivousHeartBeats;


        #region event

        public event Action<Socket> ClientHooked;

        public event Action PlcRegisterRequested;
        public event Action PlcResetRequested;
        public event Action PlcRequestToEnterNewRun;
        public event Action PlcContinueRequested;
        public event Action PlcStartRequested;
        public event Action PlcStopRequested;
        public event Action PlcResetFinished;
        public event Action<int> CustomCommandReceived;
        public event Action<long> WarningMessageReceived;
        public event Action PlcStopFinished;
        public event Action PlcPauseRequested;
        public event Action MessageSendFailed;
        public event Action HeartBeatReceived;
        public event Action<bool> IsAutoRunningChanged;
        public event Action<PlcMessagePack> MessagePackReceived;
        public event Action<PlcMessagePack> MessagePackSent;

        #endregion

        

        #region prop
        
        public bool IsPausing { get; set; }

        public IPlcErrorParser ErrorParser { get; set; }
        public IPAddress IpAddress { get; set; }

        public bool IsContinueAllowed { get; set; } = true;

        public DateTime LastHeartBeatTime { get; set; }
        

        public MachineState CurrentMachineState { get; set; }

        public int Port { get; set; }

        public bool IsConnected { get; set; }


        private bool _isBusyResetting;

        public bool IsBusyResetting
        {
            get { return _isBusyResetting; }
            set
            {
                _isBusyResetting = value;
                if (_isBusyResetting) IsAutoRunning = false;
            }
        }

        private bool _isAutoRunning;

        public bool IsAutoRunning
        {
            get { return _isAutoRunning; }
            set
            {
                _isAutoRunning = value;
                OnIsAutoRunningChanged(value);
            }
        }

        public bool IsStopping { get; set; }

        #endregion

        #region api

        public void Stop()
        {
            SentToPlc(PlcMessagePack.StopMessage, PlcMessageType.Request);
            IsStopping = true;
            CurrentMachineState = MachineState.Stopping;
        }
        
        public void SentToPlc(PlcMessagePack messagePack, PlcMessageType messageType = PlcMessageType.Respond)
        {
            var output = messagePack;
            output.MsgType = (int) messageType;
            try
            {
                _clientSocket.Send(output.ToBytes());
                OnMessagePackSent(messagePack);
            }
            catch
            {
                FailedAction();
            }
        }


        public void Start()
        {
            SentToPlc(PlcMessagePack.StartMessage, PlcMessageType.Request);
            IsAutoRunning = true;
            CurrentMachineState = MachineState.Running;
        }

        public void Continue()
        {
            SentToPlc(PlcMessagePack.ContinueMessage, PlcMessageType.Request);
            IsPausing = false;
            CurrentMachineState = MachineState.Running;
        }

        public void Pause()
        {
            SentToPlc(PlcMessagePack.PauseMessage, PlcMessageType.Request);
            IsPausing = true;
            CurrentMachineState = MachineState.Pausing;
        }

        public void ResetStates()
        {
            IsBusyResetting = false;
            IsConnected = false;
            CurrentMachineState = MachineState.Disconnect;
            IsPausing = false;
            IsStopping = false;
            IsAutoRunning = false;
        }

        public async Task ConnectAsync()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IpAddress, Port));
            _serverSocket.Listen(1);
            await Task.Run(ListenForClientHook);
        }

        /// <summary>
        /// Tell plc ready to start next loop
        /// because processing of previous loop is finished
        /// </summary>
        public void Disconnect()
        {
            ResetStates();
            _clientSocket?.Close();
            _serverSocket?.Close();
        }
        
        public void ResetPlc()
        {
            // Clear states
            IsAutoRunning = false;
            IsPausing = false;

            SentToPlc(PlcMessagePack.ResetMessage, PlcMessageType.Request);
            IsBusyResetting = true;
            CurrentMachineState = MachineState.Resetting;
        }

        #endregion



        #region ctor

        public AlcServerViewModel()
        {
            // Todo: extract command assignment to application layer
//            ConnectCommand =
//                new SimpleCommand(
//                    async o => await ExpressionHelper.RunOnlySingleFireIsAllowedEachTimeCommand(() => IsListening,
//                        ConnectAsync),
//                    o => !IsConnected);
//            DisconnectCommand = new SimpleCommand(o => Disconnect(), o => IsConnected);
//
//
//            ResetCommand = new SimpleCommand(o => ResetPlc(),
//                o => IsConnected && !IsBusyResetting && !IsAutoRunning && !IsStopping);
//            StartCommand = new SimpleCommand(o => Start(),
//                o => IsConnected && !IsBusyResetting && !IsAutoRunning && !IsStopping);
//            StopCommand = new SimpleCommand(o => Stop(),
//                o => IsConnected && !IsBusyResetting && IsAutoRunning && !IsStopping);
//            PauseCommand = new SimpleCommand(o => Pause(),
//                o => IsConnected && !IsBusyResetting && !IsPausing && !IsStopping);
//            ContinueCommand = new SimpleCommand(o => Continue(),
//                o => IsConnected && !IsBusyResetting && IsPausing && !IsStopping);

            PlcResetRequested += ResetPlc;

            PlcResetFinished += async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                IsBusyResetting = false;
            };

            PlcResetFinished += () =>
            {
                IsAutoRunning = false;
                CurrentMachineState = MachineState.Idle;
            };

            PlcStopFinished += () =>
            {
                IsAutoRunning = false;
                IsStopping = false;
            };

            IsAutoRunningChanged += isAutoRunning =>
            {
                CurrentMachineState = isAutoRunning ? MachineState.Running : MachineState.Idle;
            };

            PlcStartRequested += Start;
            PlcStopRequested += Stop;
            PlcContinueRequested += () =>
            {
                if (IsContinueAllowed) Continue();
            };
            PlcPauseRequested += Pause;

            HeartBeatReceived += UpdateLastHeartbeatTime;
        }


        
        #endregion

        #region impl

        private void UpdateLastHeartbeatTime()
        {
            LastHeartBeatTime = DateTime.Now;
        }

        private void FailedAction()
        {
            ResetStates();
            OnMessageSendFailed();
        }



       

        private void ListenForClientHook()
        {
            _clientSocket = _serverSocket.Accept();
            IsConnected = true;
            IsAutoRunning = false;
            CurrentMachineState = MachineState.Idle;

            OnNewClientHooked(_clientSocket);
            Task.Run(() => CommunicationLoop(_clientSocket));
        }

        private void CommunicationLoop(Socket clientSocket)
        {
            while (true)
            {
                if (!IsConnected) break;

                var buffer = new byte[72];
                int byteCount = 0;
                try
                {
                    byteCount = clientSocket.Receive(buffer);
                }
                catch (SocketException)
                {
                    FailedAction();
                    break;
                }

                // If no message received, end communication loop
                if (byteCount == 0) break;

                if (byteCount != 72) continue;
                var messagePack = PlcMessagePack.FromBytes(buffer);
                OnMessagePackReceived(messagePack);
                Task.Run(() => { SwitchCommandID(messagePack); });
            }
        }

        private void SwitchCommandID(PlcMessagePack messagePack)
        {
            switch (messagePack.CommandId)
            {
                case 1:
                    SentToPlc(messagePack);
                    OnPlcRegisterRequested();
                    break;
                case 2:
                    SentToPlc(messagePack);
                    OnPlcResetFinished();
                    break;
                case 3:
                    SentToPlc(messagePack);
                    OnPlcResetRequested();
                    break;
                case 5:
                    SentToPlc(messagePack);
                    OnPlcStartRequested();
                    break;
                case 6:
                    OnPlcStopFinished();
                    break;
                case 7:
                    SentToPlc(messagePack);
                    OnPlcStopRequested();
                    break;
                case 9:
                    OnPlcPauseRequested();
                    break;
                case 11:
                    OnPlcContinueRequested();
                    break;
                case 12:
                    SentToPlc(messagePack);
                    OnWarningMessageReceived(messagePack.Errorcode);
                    break;
                case 14:
                    OnHeartBeatReceived();
                    break;
                case 23:
                    OnPlcRequestToEnterNewRun();
                    break;

                default:
                    OnCustomCommandReceived(messagePack.CommandId);
                    break;
            }
        }


        #endregion

        #region invocator

        private void OnPlcResetFinished()
        {
            PlcResetFinished?.Invoke();
        }

        private void OnNewClientHooked(Socket clientSocket)
        {
            ClientHooked?.Invoke(clientSocket);
        }


        private void OnPlcRegisterRequested()
        {
            PlcRegisterRequested?.Invoke();
        }

        private void OnPlcRequestToEnterNewRun()
        {
            PlcRequestToEnterNewRun?.Invoke();
        }

        private void OnPlcStopRequested()
        {
            PlcStopRequested?.Invoke();
        }

        private void OnPlcResetRequested()
        {
            PlcResetRequested?.Invoke();
        }

        private void OnCustomCommandReceived(int commandId)
        {
            CustomCommandReceived?.Invoke(commandId);
        }


        private void OnWarningMessageReceived(long errorCode)
        {
            ErrorParser?.ParseErrorCode(errorCode);
            WarningMessageReceived?.Invoke(errorCode);
        }

        private void OnPlcStopFinished()
        {
            PlcStopFinished?.Invoke();
        }

        private void OnPlcPauseRequested()
        {
            PlcPauseRequested?.Invoke();
        }

        private void OnPlcContinueRequested()
        {
            PlcContinueRequested?.Invoke();
        }
        
        private void OnIsAutoRunningChanged(bool isAutoRunning)
        {
            IsAutoRunningChanged?.Invoke(isAutoRunning);
        }



        private void OnPlcStartRequested()
        {
            PlcStartRequested?.Invoke();
        }

        private void OnMessageSendFailed()
        {
            MessageSendFailed?.Invoke();
        }

        private void OnMessagePackReceived(PlcMessagePack messagePack)
        {
            MessagePackReceived?.Invoke(messagePack);
        }

        private void OnMessagePackSent(PlcMessagePack obj)
        {
            MessagePackSent?.Invoke(obj);
        }

        private void OnHeartBeatReceived()
        {
            HeartBeatReceived?.Invoke();
        }

        #endregion
    }
}