
using SQLite;
using System;

namespace PersonalFinanceTracker.Models
{
    public enum TransactionType
    {
        Expense,
        Income
    }
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public TransactionType Type { get; set; }

        public string Category { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}
