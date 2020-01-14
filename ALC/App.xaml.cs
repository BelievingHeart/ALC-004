using System.Windows;
using ALC.Core.ViewModels;
using ALC.Core.ViewModels.Application;

namespace ALC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Set up IoC and manage window start up myself
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            ApplicationViewModel.Init();
            
            // Start main window
            var window = new MainWindow();
            Current.MainWindow = window;
            window.Show();
        }
    }
}