using System;
using System.Windows;
using System.Windows.Input;
using ALC.Core.Command;
using ALC.Core.Helpers;
using ALC.Core.ViewModels.Message;
using ALC.Core.ViewModels.Message.Popup;
using WPFCommon.ViewModels.Message;

namespace ALC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // Ignore this
            }
        }
        

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RestoreWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }

        private void ResizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseMainWindow(object sender, RoutedEventArgs e)
        {
            var popup = new PopupViewModel
            {
                OkCommand = new CloseDialogAttachedCommand(o=>true, Close),
                CancelCommand = new CloseDialogAttachedCommand(o=>true, () => {}),
                OkButtonText = "确定",
                CancelButtonText = "取消",
                IsSpecialPopup = true,
                MessageItem = LoggingMessageItem.CreateMessage("是否退出ALC?")
            };
            Logger.EnqueuePopup(popup);
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void CleanupApplication(object sender, EventArgs e)
        {
        }
    }
}