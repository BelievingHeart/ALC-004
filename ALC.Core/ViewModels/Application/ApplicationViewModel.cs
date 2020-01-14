using ALC.Core.Constants;
using ALC.Core.ViewModels.Message;
using WPFCommon.Helpers;
using WPFCommon.ViewModels.Base;

namespace ALC.Core.ViewModels.Application
{
    public class ApplicationViewModel : ViewModelBase
    {
        private static ApplicationViewModel _instance;
        public static ApplicationViewModel Instance => _instance;


        public ApplicationViewModel()
        {
            // Load application configs and start auto-serialization
            ApplicationConfigViewModel.Instance =
                AutoSerializableHelper.LoadAutoSerializable<ApplicationConfigViewModel>(
                    DirectoryConstants.ConfigBaseDir, "AppConfigs");
            ApplicationConfigViewModel.Instance.ShouldAutoSerialize = true;

            Logger.StartPopupQueue();
            
        }

        #region api

        public static void Init()
        {
            _instance = new ApplicationViewModel();
        }

        #endregion
    }
}