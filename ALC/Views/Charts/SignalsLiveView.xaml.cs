using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts.Geared;
using LiveCharts.Wpf;

namespace ALC.Views.Charts
{
    /// <summary>
    /// Display signal curves from Precitec a controller
    /// </summary>
    public partial class SignalsLiveView : UserControl
    {

        private Dictionary<string, CartesianChart> _chartDict = new Dictionary<string, CartesianChart>();

        #region ctor

        public SignalsLiveView()
        {
            InitializeComponent();
        }

        #endregion

        #region TitlesProperty


        public static readonly DependencyProperty TitlesProperty = DependencyProperty.Register(
            "Titles", typeof(IList<string>), typeof(SignalsLiveView), new PropertyMetadata(default(IList<string>), OnTitlesChanged));

        private static void OnTitlesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (SignalsLiveView) d;
            var newTitles = (IList<string>) e.NewValue;
            if (newTitles == null || newTitles.Count == 0) return;
            
            // Add cartesian charts vertically for each title in titles
            var grid = view.PART_Grid;
            for (var index = 0; index < newTitles.Count; index++)
            {
                var title = newTitles[index];
                grid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)});
                var axisY = new AxesCollection {new Axis() {Title = title}};
                var chart = new CartesianChart() {AxisY = axisY};
                grid.Children.Add(chart);
                Grid.SetRow(chart, index);
                // Add to dictionary for convenience when updating chart values
                view._chartDict[newTitles[index]] = chart; 
            }
        }

        public IList<string> Titles
        {
            get { return (IList<string>) GetValue(TitlesProperty); }
            set { SetValue(TitlesProperty, value); }
        }

        #endregion

        #region YValuesProperty

        public static readonly DependencyProperty YValuesProperty = DependencyProperty.Register(
            "YValues", typeof(Dictionary<string, double[]>), typeof(SignalsLiveView), new PropertyMetadata(default(Dictionary<string, double[]>), OnYValuesChanged));

        private static void OnYValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (SignalsLiveView) d;
            var newYValues = (Dictionary<string, double[]>) e.NewValue;
            if (view._chartDict.Count == 0 || newYValues == null) return;
            if(view._chartDict.Count != newYValues.Count) throw new InvalidOperationException("view._chartDict.Count != newYValues.Count");

            foreach (var title in newYValues.Keys)
            {
                var chart = view._chartDict[title];
                chart.Series.Clear();
                chart.Series.Add(new GLineSeries
                {
                    Values = newYValues[title].AsGearedValues().WithQuality(Quality.Low),
                    Fill = Brushes.Transparent,
                    PointGeometry = null //use a null geometry when you have many series
                });
            }
        }

        public Dictionary<string, double[]> YValues
        {
            get { return (Dictionary<string, double[]>) GetValue(YValuesProperty); }
            set { SetValue(YValuesProperty, value); }
        }

        #endregion
    }
}