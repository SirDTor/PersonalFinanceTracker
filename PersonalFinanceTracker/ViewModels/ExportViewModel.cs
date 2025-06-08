using DocumentFormat.OpenXml.Wordprocessing;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Maui;
using Microsoft.Maui.Storage;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using static iText.Kernel.Font.PdfFontFactory;
using Cell = iText.Layout.Element.Cell;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using Style = iText.Layout.Style;
using Table = iText.Layout.Element.Table;
using TextAlignment = iText.Layout.Properties.TextAlignment;

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

        public ICommand ExportToPDFCommand { get; }

        public ICommand ExportToXLSXCommand { get; }

        public ExportViewModel()
        {
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));

            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());
            ExportToPDFCommand = new Command(async () => await ExportToPdfAsync());
            ExportToXLSXCommand = new Command(async () => await ExportToExcelAsync());

            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
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

        private async Task ExportToExcelAsync()
        {
            try
            {
                var start = StartDate.Date;
                var end = StartDate.Date.AddDays(1).AddTicks(-1);

                var transactions = _dataService
                    .GetAll()
                    .Where(t => t.Date >= start && t.Date <= end)
                    .ToList();

                if (!transactions.Any())
                {
                    ExportStatus = "Нет транзакций за выбранный период.";
                    return;
                }

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Транзакции");

                // Заголовки
                worksheet.Cell(1, 1).Value = "Дата";
                worksheet.Cell(1, 2).Value = "Категория";
                worksheet.Cell(1, 3).Value = "Описание";
                worksheet.Cell(1, 4).Value = "Сумма";
                worksheet.Cell(1, 5).Value = "Тип";

                for (int i = 0; i < transactions.Count; i++)
                {
                    var t = transactions[i];
                    worksheet.Cell(i + 2, 1).Value = t.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(i + 2, 2).Value = t.Category;
                    worksheet.Cell(i + 2, 3).Value = t.Description;
                    worksheet.Cell(i + 2, 4).Value = t.Amount;
                    worksheet.Cell(i + 2, 5).Value = GetTransactionTypeDisplay(t.Type);
                }

                var fileName = $"Transactions_{start:yyyyMMdd}_{end:yyyyMMdd}.xlsx";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                workbook.SaveAs(filePath);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Экспорт транзакций (Excel)",
                    File = new ShareFile(filePath)
                });

                ExportStatus = "Экспорт в Excel завершён.";
            }
            catch (Exception ex)
            {
                ExportStatus = $"Ошибка Excel: {ex.Message}";
            }
        }

        private async Task ExportToPdfAsync()
        {
            string fileName = $"Transactions_{DateTime.Now:yyyyMMddHHmmss}.pdf";

#if ANDROID
    var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
    var filePath = Path.Combine(docsDirectory.AbsolutePath, fileName);
#else
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
#endif

            var start = StartDate.Date;
            var end = EndDate.Date.AddDays(1).AddTicks(-1);

            var transactions = _dataService
                .GetAll()
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date) // Добавим сортировку по дате
                .ToList();

            if (!transactions.Any())
            {
                ExportStatus = "Нет транзакций за выбранный период.";
                return;
            }

            try
            {
                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    PdfFont font;
                    using (var stream = await FileSystem.OpenAppPackageFileAsync("Roboto-Regular.ttf"))
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        font = PdfFontFactory.CreateFont(ms.ToArray(), PdfEncodings.IDENTITY_H, EmbeddingStrategy.FORCE_EMBEDDED);
                    }

                    document.SetFont(font);

                    // Заголовок документа
                    Paragraph header = new Paragraph($"Отчёт транзакций с {start:dd.MM.yyyy} по {end:dd.MM.yyyy}")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18);
                    document.Add(header);
                    document.Add(new LineSeparator(new SolidLine()));
                    document.Add(new Paragraph("\n"));

                    // Таблица с явно заданными ширинами столбцов
                    float[] columnWidths = { 1.5f, 1.5f, 2f, 1.5f, 1f };
                    Table table = new Table(UnitValue.CreatePercentArray(columnWidths))
                        .UseAllAvailableWidth();

                    // Стиль для заголовков
                    Style headerStyle = new Style()
                        .SetBackgroundColor(new DeviceRgb(70, 130, 180))
                        .SetFontColor(DeviceRgb.WHITE);

                    // Добавляем заголовки
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Дата").AddStyle(headerStyle)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Категория").AddStyle(headerStyle)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Описание").AddStyle(headerStyle)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Сумма").AddStyle(headerStyle)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Тип").AddStyle(headerStyle)));

                    // Добавляем данные
                    foreach (var t in transactions)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(t.Date.ToString("dd.MM.yyyy"))));
                        table.AddCell(new Cell().Add(new Paragraph(t.Category ?? "-")));
                        table.AddCell(new Cell().Add(new Paragraph(t.Description ?? "-")));
                        table.AddCell(new Cell().Add(new Paragraph(t.Amount.ToString("F2"))));
                        table.AddCell(new Cell().Add(   new Paragraph(GetTransactionTypeDisplay(t.Type) ?? "-")));
                    }

                    document.Add(table);

                    // Добавляем итоговую информацию
                    decimal totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                    decimal totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                    decimal balance = totalIncome - totalExpense;

                    document.Add(new Paragraph("\n")); // Отступ перед итогами
                    document.Add(new Paragraph($"Всего доходов: {totalIncome:F2}")
                        .SetTextAlignment(TextAlignment.RIGHT));
                    document.Add(new Paragraph($"Всего расходов: {totalExpense:F2}")
                        .SetTextAlignment(TextAlignment.RIGHT));
                    document.Add(new Paragraph($"Баланс: {balance:F2}")
                        .SetTextAlignment(TextAlignment.RIGHT));

                    document.Close();
                }

                // Проверяем, существует ли файл перед попыткой поделиться
                if (File.Exists(filePath))
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = "Поделиться PDF",
                        File = new ShareFile(filePath)
                    });

                    ExportStatus = "Экспорт в PDF завершён и файл отправлен.";
                }
                else
                {
                    ExportStatus = "Ошибка: PDF файл не был создан.";
                }
            }
            catch (Exception ex)
            {
                ExportStatus = $"Ошибка при экспорте: {ex.Message}";
                // Для отладки можно добавить логирование
                Debug.WriteLine($"Ошибка экспорта PDF: {ex}");
            }
        }

        private string GetTransactionTypeDisplay(TransactionType type)
        {
            return type switch
            {
                TransactionType.Expense => "Расход",
                TransactionType.Income => "Доход",
                _ => type.ToString()
            };
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
