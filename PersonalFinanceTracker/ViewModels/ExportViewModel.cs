using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace PersonalFinanceTracker.ViewModels
{
    public class ExportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DatabaseService _dataService;

        private string _exportStatus;
        public string ExportStatus
        {
            get => _exportStatus;
            set => SetProperty(ref _exportStatus, value);
        }        

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public ICommand ExportToCsvCommand { get; }
        public ICommand ExportToTxtCommand { get; }
        public ICommand ExportCommand { get; }

        public ExportViewModel()
        {
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));

            ExportToCsvCommand = new Command(ExportToCsv);
            ExportToTxtCommand = new Command(ExportToTxt);
            ExportCommand = new Command(async () => await ExportToCsvAsync());

            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
        }

        private void ExportToCsv()
        {
            var transactions = _dataService.GetAll();
            var path = Path.Combine(FileSystem.AppDataDirectory, $"export_{DateTime.Now:yyyyMMddHHmmss}.csv");

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("Дата,Категория,Тип,Сумма,Описание");
                foreach (var t in transactions)
                {
                    writer.WriteLine($"{t.Date:d},{t.Category},{t.Type},{t.Amount},{t.Description}");
                }
            }

            ExportStatus = $"CSV экспортирован: {path}";
        }

        private async Task ExportToCsvAsync()
        {
            try
            {
                var start = StartDate.Date;
                var end = EndDate.Date.AddDays(1).AddTicks(-1);

                var transactions = _dataService
                    .GetAll()
                    .Where(t => t.Date >= start && t.Date <= end)
                    .ToList();

                if (!transactions.Any())
                {
                    ExportStatus = "Нет транзакций за выбранный период.";
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("Дата,Категория,Описание,Сумма,Тип");

                foreach (var t in transactions)
                {
                    sb.AppendLine($"{t.Date:yyyy-MM-dd},{t.Category},{t.Description},{t.Amount},{t.Type}");
                }

                var fileName = $"Transactions_{start:yyyyMMdd}_{end:yyyyMMdd}.csv";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Экспорт транзакций",
                    File = new ShareFile(filePath)
                });

                ExportStatus = "Экспорт завершён.";
            }
            catch (Exception ex)
            {
                ExportStatus = $"Ошибка: {ex.Message}";
            }
        }

        private void ExportToTxt()
        {
            var transactions = _dataService.GetAll();
            var path = Path.Combine(FileSystem.AppDataDirectory, $"export_{DateTime.Now:yyyyMMddHHmmss}.txt");

            using (var writer = new StreamWriter(path))
            {
                foreach (var t in transactions)
                {
                    writer.WriteLine($"{t.Date:d} | {t.Category} | {t.Type} | {t.Amount}₽ | {t.Description}");
                }
            }

            ExportStatus = $"TXT экспортирован: {path}";
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
