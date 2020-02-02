using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Threading;

namespace Precitec
{
    public class CHR2Controller
    {
        private readonly string[] _signalNames;
        private uint _deviceHandle;
        private DispatcherTimer _timer;
        private List<double[]> _sampleBuffers;
        private double[] _oneSampleData;
        private int _currentDataPos;
        private readonly int _dataLength = 1024;
        private short[] _specData;


        public event Action<Dictionary<string, double[]>> CurvesUpdated;
        public event Action<short[]> SpectrumDataUpdated; 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress">ip address of the precitec device</param>
        /// <param name="signalIds">the signals to output</param>
        /// <param name="signalNames">the signal names corresponds to signal ids</param>
        /// <param name="updateInterval">The interval to read data from precitec device</param>
        /// <exception cref="InvalidOperationException"></exception>
        public CHR2Controller(string ipAddress, int[] signalIds, string[] signalNames, int updateInterval = 50)
        {
            _signalNames = signalNames;
            // Connect
            var connectionInfo = "IP:" + ipAddress;
            var connectSuccess = TCHRDLLFunctionWrapper.OpenConnection(connectionInfo, TCHRDLLFunctionWrapper.CHR_2Gen_Device, out var connectionId) == 0;
            if(!connectSuccess) throw new InvalidOperationException($"Connection to CHR2 device {ipAddress} failed");
            _deviceHandle = Convert.ToUInt32(connectionId);
            
            // Set up signals
            var signalSetupSuccess = TCHRDLLFunctionWrapper.SetOutputSignals(_deviceHandle, signalIds, signalIds.Length) == 0;
           if(!signalSetupSuccess) throw new InvalidOperationException("Signal setup failed");
           _oneSampleData = new double[signalIds.Length];
           _sampleBuffers = new List<double[]>();
           var bufferLength = 1024;
           for (int i = 0; i < signalIds.Length; i++)
           {
               _sampleBuffers.Add(new double[bufferLength]);
           }
           _specData = new short[_dataLength];
           
           // Set up timer
           _timer = new DispatcherTimer(DispatcherPriority.Normal);
           _timer.Tick += TimerOnElapsed;
           _timer.Interval = TimeSpan.FromMilliseconds(updateInterval);
           _timer.Start();
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
                IntPtr pData = IntPtr.Zero;
            //read in CHRocodile data
            while (true)
            {
     
                
                 var  res = TCHRDLLFunctionWrapper.GetNextSample(_deviceHandle, ref pData, out var sigNumber);
                
                if (res > 0)
                {
                    //display data
                    Marshal.Copy(pData, _oneSampleData, 0, sigNumber);
        
                    if (sigNumber > 0)
                    {
                        for (int i = 0; i < sigNumber; i++)
                        {
                            _sampleBuffers[i][_currentDataPos] = _oneSampleData[i];
                        }
                        _currentDataPos++;
                        if (_currentDataPos >= _dataLength)
                            _currentDataPos = 0;
                    }
                }
                else
                {
                    if (res < 0)
                        TCHRDLLFunctionWrapper.FlushInputBuffer(_deviceHandle);
                    break;
                }
            }

            // Convert to dictionary
            var curvesDict = new Dictionary<string, double[]>();
            for (int i = 0; i < _signalNames.Length; i++)
            {
                curvesDict[_signalNames[i]] = _sampleBuffers[i];
            }
            
            OnCurvesUpdated(curvesDict);

            //download spectrum
                int SpecType = TCHRDLLFunctionWrapper.Raw_Spectrum;
                if (TCHRDLLFunctionWrapper.DownloadDeviceSpectrum(_deviceHandle, SpecType, ref pData, out var signalCount) == 0)
                {
                    if (signalCount > _dataLength)
                        signalCount = _dataLength;
                    Marshal.Copy(pData, _specData, 0, signalCount);
                    // Assign 0s to unassigned entries
                    for (int i = signalCount; i < _dataLength; i++)
                        _specData[i] = 0;

                    OnSpectrumDataUpdated(_specData);
                }
            
        }

        public double[] GetLastSample()
        {
            var ptr = IntPtr.Zero;
            TCHRDLLFunctionWrapper.GetLastSample(_deviceHandle, ref ptr, out var numSignals);
            Marshal.Copy(ptr, _oneSampleData, 0, numSignals);

            return _oneSampleData;
        }


        protected virtual void OnCurvesUpdated(Dictionary<string, double[]> obj)
        {
            CurvesUpdated?.Invoke(obj);
        }

        protected virtual void OnSpectrumDataUpdated(short[] obj)
        {
            SpectrumDataUpdated?.Invoke(obj);
        }
    }
}