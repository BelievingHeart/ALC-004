using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ALC.Core.Constants;
using ALC.Core.Enums;
using ALC.Core.ViewModels;
using WPFCommon.Commands;
using WPFCommon.Helpers;
using WPFCommon.ViewModels.Base;

namespace ALC.Core
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static MainWindowViewModel _instance = new MainWindowViewModel()
        {
            CurrentApplicationPage = ApplicationPage.Home
        };
        public static MainWindowViewModel Instance => _instance;

        public ApplicationPage CurrentApplicationPage { get; set; }
        
          #region Commands

        public ICommand SwitchHomePageCommand { get; set; }
        public ICommand SwitchCameraPageCommand { get; set; }
        public ICommand SwitchLaserPageCommand { get; set; }
        public ICommand SwitchServerPageCommand { get; set; }

        public ICommand SwitchSettingsPageCommand { get; }

        public ICommand SwitchTablePageCommand { get; }
        public ICommand SwitchChartPageCommand { get; }

        public ICommand SwitchLoginPageCommand { get; }

        public ICommand OpenHelpDocsCommand { get; }
        #endregion

        #region Command Implemtations

        private void SwitchHomePage()
        {
            CurrentApplicationPage = ApplicationPage.Home;
        }

        private void SwitchCameraPage()
        {
            CurrentApplicationPage = ApplicationPage.Camera;
        }

        private void SwitchLaserPage()
        {
            CurrentApplicationPage = ApplicationPage.Laser;
        }

        private void SwitchServerPage()
        {
            CurrentApplicationPage = ApplicationPage.Server;
        }
        
        private void SwitchSettingsPage()
        {
            CurrentApplicationPage = ApplicationPage.Settings;
        }
        
        private void SwitchTablePage()
        {
            CurrentApplicationPage = ApplicationPage.Table;
        }
        
        private void SwitchLoginPage()
        {
            CurrentApplicationPage = ApplicationPage.Login;
        }

        private void SwitchChartPage()
        {
            CurrentApplicationPage = ApplicationPage.Charts;
        }
        #endregion


        #region ctor

        public MainWindowViewModel()
        {
            SwitchHomePageCommand = new SimpleCommand(o => SwitchHomePage(),
                o => CurrentApplicationPage != ApplicationPage.Home);
            SwitchCameraPageCommand = new SimpleCommand(o => SwitchCameraPage(),
                o => CurrentApplicationPage != ApplicationPage.Camera);
            SwitchLaserPageCommand = new SimpleCommand(o => SwitchLaserPage(),
                o => CurrentApplicationPage != ApplicationPage.Laser);
            SwitchServerPageCommand = new SimpleCommand(o => SwitchServerPage(),
                o => CurrentApplicationPage != ApplicationPage.Server);
            SwitchSettingsPageCommand = new SimpleCommand(o => SwitchSettingsPage(),
                o => CurrentApplicationPage != ApplicationPage.Settings);
            SwitchTablePageCommand = new SimpleCommand(o => SwitchTablePage(),
                o => CurrentApplicationPage != ApplicationPage.Table);
            SwitchLoginPageCommand = new SimpleCommand(o => SwitchLoginPage(),
                o => CurrentApplicationPage != ApplicationPage.Login);
            SwitchChartPageCommand = new SimpleCommand(o=> SwitchChartPage(), 
                o=>CurrentApplicationPage!=ApplicationPage.Charts);
            OpenHelpDocsCommand = new RelayCommand(()=>Process.Start(Path.Combine(DirectoryConstants.DocumentaryDir, "IA906调试说明书.docx")));
        }



        #endregion


    }
}