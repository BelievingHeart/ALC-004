using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using ALC.Core.Enums;
using ALC.Views.ApplicationPage;
using WPFCommon.Converters;

namespace ALC.Converters
{
    public class ApplicationPageConverter : ValueConverterBase<ApplicationPageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (ApplicationPage) value;
            Type pageType;
            switch (input)
            {
                case ApplicationPage.Home:
                    pageType = typeof(HomeView);
                    break;
                case ApplicationPage.Login:
                    pageType = typeof(LoginView);
                    break;
                case ApplicationPage.Camera:
                    pageType = typeof(CameraView);
                    break;
                case ApplicationPage.Laser:
                    pageType = typeof(LaserView);
                    break;
                case ApplicationPage.Table:
                    pageType = typeof(TableView);
                    break;
                case ApplicationPage.Server:
                    pageType = typeof(ServerView);
                    break;
                case ApplicationPage.Settings:
                    pageType = typeof(SettingsView);
                    break;
                default:
                    throw new NotSupportedException("Can not find such type of page");
            }

            if (_pages.Any(ele => ele.GetType() == pageType))
            {
                return _pages.First(ele => ele.GetType() == pageType);
            }
            else
            {
                var page = (UserControl)Activator.CreateInstance(pageType);
                _pages.Add(page);

                return page;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
        
        private static IList<UserControl> _pages = new List<UserControl>();
    }
}