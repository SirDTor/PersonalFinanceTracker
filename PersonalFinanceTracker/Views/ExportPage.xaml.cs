using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace PersonalFinanceTracker.Views
{
    public partial class ExportPage : ContentPage
    {
        private readonly DatabaseService _dataService;

        public ExportPage()
        {
            InitializeComponent();
            _dataService = new DatabaseService(Path.Combine(FileSystem.AppDataDirectory, "financedb.db"));
        }

        private async void OnExportClicked(object sender, EventArgs e)
        {
            try
            {
                var start = StartDatePicker.Date.Date;
                var end = EndDatePicker.Date.Date.AddDays(1).AddTicks(-1);

                List<Transaction> transactions = _dataService
                    .GetAll()
                    .Where(t => t.Date >= start && t.Date <= end)
                    .ToList();

                if (!transactions.Any())
                {
                    StatusLabel.Text = "��� ���������� �� ��������� ������.";
                    return;
                }

                // ��������� CSV
                var sb = new StringBuilder();
                sb.AppendLine("����,���������,��������,�����,���");

                foreach (var t in transactions)
                {
                    sb.AppendLine($"{t.Date:yyyy-MM-dd},{t.Category},{t.Description},{t.Amount},{t.Type}");
                }

                // ��� ����� � ���� �� ��������� �����
                var fileName = $"Transactions_{start:yyyyMMdd}_{end:yyyyMMdd}.csv";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // ��������� CSV
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

                // ��������� "����������"
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "������� ����������",
                    File = new ShareFile(filePath)
                });

                StatusLabel.Text = "������� ��������.";
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", $"�� ������� �������������� ������: {ex.Message}", "��");
            }
        }

    }
}
