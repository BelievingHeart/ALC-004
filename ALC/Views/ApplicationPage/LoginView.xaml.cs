using System.Windows;
using System.Windows.Controls;
using ALC.Core.ViewModels;
using ALC.Core.ViewModels.Application;

namespace ALC.Views.ApplicationPage
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }
        
        private void InputPasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = (LoginViewModel) DataContext;
            var passwordBox = (PasswordBox) sender;
            viewModel.InputPassWord = passwordBox.Password;
        }

        private void OldPasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = (LoginViewModel) DataContext;
            var passwordBox = (PasswordBox) sender;
            viewModel.InputPassWord = passwordBox.Password;
        }

        private void NewPasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = (LoginViewModel) DataContext;
            var passwordBox = (PasswordBox) sender;
            viewModel.NewPassword = passwordBox.Password;
        }

        private void NewPasswordBoxDoubleCheckPasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = (LoginViewModel) DataContext;
            var passwordBox = (PasswordBox) sender;
            viewModel.NewPasswordDoubleCheck = passwordBox.Password;
        }

    

        private void ClearPassword(object sender, RoutedEventArgs e)
        {
            InputPasswordBox.Clear();
        }
    }
}