
using SQLite;
using PersonalFinanceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinanceTracker.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _db;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Transaction>();
        }

        public List<Transaction> GetAll() => _db.Table<Transaction>().OrderByDescending(t => t.Date).ToList();

        public void Add(Transaction transaction) => _db.Insert(transaction);

        public void Delete(Transaction transaction) => _db.Delete(transaction);

        public decimal GetBalance() =>
            _db.Table<Transaction>().Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount)
            - _db.Table<Transaction>().Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

    }
}
