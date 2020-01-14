using ALC.Core.Command;
using ALC.Core.ViewModels.Message.Popup;
using PLCCommunication.Core;
using PLCCommunication.Core.Enums;
using PLCCommunication.Core.ViewModels;
using WPFCommon.ViewModels.Message;

namespace ALC.Core.Helpers
{
    public static class PopupHelper
    {
        public static PopupViewModel CreateNormalPopup(string message)
        {
          return  new PopupViewModel()
            {
                OkButtonText = "确定",
                CancelButtonText = "取消",
                OkCommand = new CloseDialogAttachedCommand(o => true, () => {}),
                CancelCommand = new CloseDialogAttachedCommand(o => true, () => {}),
                MessageItem = LoggingMessageItem.CreateMessage(message),
                IsSpecialPopup = false
            };
        }

        public static PopupViewModel CreateClearProductPopup(string message, long errorCode, AlcServerViewModel alcServer)
        {

            // TODO: define continue and quit message pack
            PlcMessagePack continueMessagePack = null;
            PlcMessagePack quitMessagePack = null;
            
            var content = errorCode == 2088? "请清除所有产品后点继续，或者点退出取消自动模式" : "请清料后点击继续，或者点退出再清料";
            
            return  new PopupViewModel()
            {
                OkButtonText = "继续",
                CancelButtonText = "退出",
                OkCommand = new CloseDialogAttachedCommand(o => true, () =>
                    {
                        alcServer.SentToPlc(continueMessagePack, PlcMessageType.Request);
                    }),
                CancelCommand = new CloseDialogAttachedCommand(o => true, () =>
                {
                    alcServer.SentToPlc(quitMessagePack, PlcMessageType.Request);
                    alcServer.IsAutoRunning = false;
                }),
                MessageItem = LoggingMessageItem.CreateMessage(message),
                Content = content,
                IsSpecialPopup = true
            };
        }

        public static PopupViewModel CreateSafeDoorPopup(string message, AlcServerViewModel alcServer)
        {
     

            // TODO: define continue and quit message pack
            PlcMessagePack continueMessagePack = null;
            PlcMessagePack quitMessagePack = null;
            
            var content = "请关门后点击继续，或者点取消退出自动模式";
            
            return  new PopupViewModel()
            {
                OkButtonText = "继续",
                CancelButtonText = "取消",
                OkCommand = new CloseDialogAttachedCommand(o => true, () =>
                    {
                        alcServer.SentToPlc(continueMessagePack, PlcMessageType.Request);
                    }),
                CancelCommand = new CloseDialogAttachedCommand(o => true, () =>
                {
                    alcServer.SentToPlc(quitMessagePack, PlcMessageType.Request);
                    alcServer.IsAutoRunning = false;
                }),
                MessageItem = LoggingMessageItem.CreateMessage(message),
                Content = content,
                IsSpecialPopup = true
            };        }
    }
}