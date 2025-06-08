using PersonalFinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceTracker.Services
{
    public class TransactionAmountConverter : IValueConverter
    {
        // value — decimal Amount
        // parameter — TransactionType (string или enum)

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount && parameter is TransactionType type)
            {
                string sign = type == TransactionType.Income ? "+" : "-";

                if (targetType == typeof(string))
                {
                    return $"{sign}{amount:C}";
                }
                else if (targetType == typeof(Color))
                {
                    return type == TransactionType.Income ? Colors.Green : Colors.Red;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
