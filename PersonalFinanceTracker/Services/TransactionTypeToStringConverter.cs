using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Converters
{
    public class TransactionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type switch
                {
                    TransactionType.Income => "Доход",
                    TransactionType.Expense => "Расход",
                    _ => "Неизвестно"
                };
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str switch
                {
                    "Доход" => TransactionType.Income,
                    "Расход" => TransactionType.Expense,
                    _ => TransactionType.Expense // По умолчанию
                };
            }
            return TransactionType.Expense;
        }
    }
}
