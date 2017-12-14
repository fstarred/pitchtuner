using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfPitchTuner.Converters
{

    [ValueConversion(typeof(double), typeof(double))]
    class BarMeterTranslateConverter : DependencyObject, IValueConverter
    {
        // doesn't work
        public static readonly DependencyProperty MeterWidthProperty =
            DependencyProperty.Register("MeterWidth", typeof(Double), typeof(BarMeterTranslateConverter), new PropertyMetadata(0.0, OnMeterWidthChanged));

        public Double MeterWidth
        {
            get { return (Double)GetValue(MeterWidthProperty); }
            set { SetValue(MeterWidthProperty, value); }
        }

        static void OnMeterWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public static readonly DependencyProperty MeterWidthResourceProperty =
            DependencyProperty.Register("MeterWidthResource", typeof(string), typeof(BarMeterTranslateConverter));

        public string MeterWidthResource
        {
            get { return (string)GetValue(MeterWidthResourceProperty); }
            set { SetValue(MeterWidthResourceProperty, value); }
        }

        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;

            double meterWidth = (double)App.Current.FindResource(MeterWidthResource);

            //double meterWidth = MeterWidth;

            Double unit = meterWidth / 100;

            val = unit * val;

            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
