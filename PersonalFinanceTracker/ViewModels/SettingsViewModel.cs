using ClosedXML.Excel;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PersonalFinanceTracker.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DatabaseService _dataService;

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand DeleteAllCommand { get; }
        public ICommand NavigateToExportCommand { get; }
        public ICommand NavigateToPinSettingsCommand { get; }
        public ICommand ImportDataCommand { get; }

        public event Action NavigateToExportRequested;
        public event Action NavigateToPinSettingsRequested;

        public SettingsViewModel()
        {
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));

            DeleteAllCommand = new Command(async () => await DeleteAllAsync());
            NavigateToExportCommand = new Command(() => NavigateToExportRequested?.Invoke());
            NavigateToPinSettingsCommand = new Command(() => NavigateToPinSettingsRequested?.Invoke());
            ImportDataCommand = new Command(async () => await ImportDataAsync());
        }

        private async Task DeleteAllAsync()
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Удаление данных", "Удалить все транзакции?", "Да", "Отмена");
            if (!confirmed) return;

            var all = _dataService.GetAll();
            foreach (var t in all)
                _dataService.Delete(t);

            StatusMessage = "Все данные удалены.";
        }

        private async Task ImportDataAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Выберите CSV или XLSX файл",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.Android, new[] { ".csv", ".xlsx" } },
                        { DevicePlatform.iOS, new[] { "public.comma-separated-values-text", "org.openxmlformats.spreadsheetml.sheet" } },
                        { DevicePlatform.WinUI, new[] { ".csv", ".xlsx" } }
                    })
                });

                if (result == null) return;

                if (result.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    await ImportCsvAsync(result.FullPath);
                }

                else if (result.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    await ImportXlsxAsync(result.FullPath);
                }

                StatusMessage = "Импорт завершён.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка при импорте: {ex.Message}";
            }
        }

        private async Task ImportCsvAsync(string path)
        {
            var lines = await File.ReadAllLinesAsync(path);

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');

                if (parts.Length < 5) continue;

                var transaction = new Transaction
                {
                    Date = DateTime.Parse(parts[0]),
                    Category = parts[1],
                    Description = parts[2],
                    Amount = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                    Type = Enum.Parse<TransactionType>(parts[4], true)
                };

                _dataService.Add(transaction);
            }
        }

        private async Task ImportXlsxAsync(string path)
        {
            await Task.Run(() =>
            {
                using var workbook = new XLWorkbook(path);
                var worksheet = workbook.Worksheets.First();

                for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    try
                    {
                        var date = worksheet.Cell(row, 1).GetDateTime();
                        var category = worksheet.Cell(row, 2).GetString();
                        var description = worksheet.Cell(row, 3).GetString();
                        var amountCell = worksheet.Cell(row, 4);
                        var typeString = worksheet.Cell(row, 5).GetString();
                        decimal amount = 0;

                        var type = typeString?.Trim().ToLower() switch
                        {
                            "Доход" => TransactionType.Income,
                            "Расход" => TransactionType.Expense,
                            _ => TransactionType.Expense
                        };

                        string amountText = amountCell.GetString().Replace(',', '.'); // заменить запятую на точку
                        if (!decimal.TryParse(amountText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out amount))
                            continue;

                        var transaction = new Transaction
                        {
                            Date = date,
                            Category = category,
                            Description = description,
                            Amount = amount,
                            Type = type
                        };

                        _dataService.Add(transaction);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Ошибка импорта строки {row}: {ex.Message}");
                    }
                }
            });

            StatusMessage = "Импорт из Excel завершён.";
        }


        protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(backingField, value))
            {
                backingField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
