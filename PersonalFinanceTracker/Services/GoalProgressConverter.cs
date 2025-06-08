using System;
using System.Globalization;

namespace PersonalFinanceTracker.Services
{
    public class GoalProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal current && parameter is string targetStr && decimal.TryParse(targetStr, out var target) && target > 0)
                return (double)(current / target);
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
