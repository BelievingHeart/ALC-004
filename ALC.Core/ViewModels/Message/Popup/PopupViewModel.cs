using ALC.Core.Command;
using WPFCommon.ViewModels.Base;
using WPFCommon.ViewModels.Message;

namespace ALC.Core.ViewModels.Message.Popup
{
    public class PopupViewModel : ViewModelBase
    {
        public CloseDialogAttachedCommand OkCommand { get; set; }
        public CloseDialogAttachedCommand CancelCommand { get; set; }
        public string OkButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public LoggingMessageItem MessageItem { get; set; }
        public string Content { get; set; }

        public bool IsSpecialPopup { get; set; }
    }
}