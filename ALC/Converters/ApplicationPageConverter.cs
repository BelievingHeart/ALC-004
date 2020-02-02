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
                case ApplicationPage.Charts:
                    pageType = typeof(ChartView);
                    break;
                default:
                    throw new NotSupportedException("Can not find such type of page");
            }

            // If page created, take from the list
            if (Pages.Any(ele => ele.GetType() == pageType))
            {
                return Pages.First(ele => ele.GetType() == pageType);
            }

            // If not created push in list and return the new page
            var page = (UserControl)Activator.CreateInstance(pageType);
            Pages.Add(page);
            return page;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
        
        private static readonly IList<UserControl> Pages = new List<UserControl>();
    }
}