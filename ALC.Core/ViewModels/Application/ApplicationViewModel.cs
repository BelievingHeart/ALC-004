using System.Collections.Generic;
using ALC.Core.Constants;
using ALC.Core.ViewModels.Message;
using Precitec;
using WPFCommon.Helpers;
using WPFCommon.ViewModels.Base;

namespace ALC.Core.ViewModels.Application
{
    public class ApplicationViewModel : ViewModelBase
    {

        #region private field

        private CHR2Controller _precitectController;
        // TODO: define ip address
        private string _precitecControllerIp;
        // TODO: define signal ids
        private int[] _signalIds;



        
        private static ApplicationViewModel _instance;

        #endregion
        public static ApplicationViewModel Instance => _instance;


        public ApplicationViewModel()
        {
            // Load application configs and start auto-serialization
            ApplicationConfigViewModel.Instance =
                AutoSerializableHelper.LoadAutoSerializable<ApplicationConfigViewModel>(
                    DirectoryConstants.ConfigBaseDir, "AppConfigs");
            ApplicationConfigViewModel.Instance.ShouldAutoSerialize = true;

            Logger.StartPopupQueue();
            
            // Setup precitec controller
            _precitectController = new CHR2Controller(_precitecControllerIp, _signalIds, SignalNames);
            _precitectController.CurvesUpdated += curves => SignalCurvesData = curves;

        }

        #region props

        public Dictionary<string, double[]> SignalCurvesData { get; set; }
        
        // TODO: define signal names
        public string[] SignalNames { get; set; }

        #endregion


        #region api

        public static void Init()
        {
            _instance = new ApplicationViewModel();
        }

        #endregion
    }
}