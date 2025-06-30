
using SQLite;
using PersonalFinanceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinanceTracker.Services
{
    public interface IDatabaseService
    {
        // Transactions
        List<Transaction> GetAllTransactions();
        Task<List<Transaction>> GetTransactionsAsync();
        List<Transaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate);
        List<Transaction> GetTransactionsByCategory(string category);
        void AddTransaction(Transaction transaction);
        void UpdateTransaction(Transaction transaction);
        void DeleteTransaction(Transaction transaction);
        decimal GetBalance();
        decimal GetBalanceByCategory(string category);
        
        // Goals
        List<Goal> GetGoals();
        Task<List<Goal>> GetGoalsAsync();
        void AddGoal(Goal goal);
        void UpdateGoal(Goal goal);
        void DeleteGoal(Goal goal);
        
        // Budgets
        List<Budget> GetBudgets();
        List<Budget> GetActiveBudgets();
        void AddBudget(Budget budget);
        void UpdateBudget(Budget budget);
        void DeleteBudget(Budget budget);
        
        // Categories
        List<Category> GetCategories();
        List<Category> GetCategoriesByType(TransactionType type);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
        
        // Recurring Transactions
        List<RecurringTransaction> GetRecurringTransactions();
        List<RecurringTransaction> GetDueRecurringTransactions();
        void AddRecurringTransaction(RecurringTransaction recurringTransaction);
        void UpdateRecurringTransaction(RecurringTransaction recurringTransaction);
        void DeleteRecurringTransaction(RecurringTransaction recurringTransaction);
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly SQLiteConnection _db;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteConnection(dbPath);
            InitializeTables();
            SeedDefaultData();
        }

        private void InitializeTables()
        {
            _db.CreateTable<Transaction>();
            _db.CreateTable<Goal>();
            _db.CreateTable<Budget>();
            _db.CreateTable<Category>();
            _db.CreateTable<RecurringTransaction>();
        }

        private void SeedDefaultData()
        {
            if (!_db.Table<Category>().Any())
            {
                var defaultCategories = new List<Category>
                {
                    new() { Name = "Еда", Icon = "🍽️", Color = "#FF6F61", TransactionType = TransactionType.Expense, IsDefault = true },
                    new() { Name = "Транспорт", Icon = "🚗", Color = "#6B5B95", TransactionType = TransactionType.Expense, IsDefault = true },
                    new() { Name = "Развлечения", Icon = "🎬", Color = "#88B04B", TransactionType = TransactionType.Expense, IsDefault = true },
                    new() { Name = "Здоровье", Icon = "🏥", Color = "#FFA07A", TransactionType = TransactionType.Expense, IsDefault = true },
                    new() { Name = "Жилье", Icon = "🏠", Color = "#F7CAC9", TransactionType = TransactionType.Expense, IsDefault = true },
                    new() { Name = "Зарплата", Icon = "💰", Color = "#6BE53E", TransactionType = TransactionType.Income, IsDefault = true },
                    new() { Name = "Инвестиции", Icon = "📈", Color = "#92A8D1", TransactionType = TransactionType.Income, IsDefault = true },
                    new() { Name = "Другое", Icon = "📝", Color = "#607D8B", TransactionType = TransactionType.Expense, IsDefault = true }
                };
                
                foreach (var category in defaultCategories)
                {
                    _db.Insert(category);
                }
            }
        }

        // Transactions
        public List<Transaction> GetAllTransactions() => 
            _db.Table<Transaction>().OrderByDescending(t => t.Date).ToList();
        
        public Task<List<Transaction>> GetTransactionsAsync() => 
            Task.FromResult(_db.Table<Transaction>().OrderByDescending(t => t.Date).ToList());

        public List<Transaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate) =>
            _db.Table<Transaction>()
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToList();

        public List<Transaction> GetTransactionsByCategory(string category) =>
            _db.Table<Transaction>()
                .Where(t => t.Category == category)
                .OrderByDescending(t => t.Date)
                .ToList();

        public void AddTransaction(Transaction transaction) => _db.Insert(transaction);
        public void UpdateTransaction(Transaction transaction) => _db.Update(transaction);
        public void DeleteTransaction(Transaction transaction) => _db.Delete(transaction);

        public decimal GetBalance()
        {
            var income = _db.Table<Transaction>().Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var expense = _db.Table<Transaction>().Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            return income - expense;
        }

        public decimal GetBalanceByCategory(string category)
        {
            var transactions = _db.Table<Transaction>().Where(t => t.Category == category);
            return transactions.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);
        }

        // Goals
        public List<Goal> GetGoals() => _db.Table<Goal>().ToList();
        public Task<List<Goal>> GetGoalsAsync() => Task.FromResult(_db.Table<Goal>().ToList());
        public void AddGoal(Goal goal) => _db.Insert(goal);
        public void UpdateGoal(Goal goal) => _db.Update(goal);
        public void DeleteGoal(Goal goal) => _db.Delete(goal);

        // Budgets
        public List<Budget> GetBudgets() => _db.Table<Budget>().ToList();
        public List<Budget> GetActiveBudgets() => _db.Table<Budget>().Where(b => b.IsActive).ToList();
        public void AddBudget(Budget budget) => _db.Insert(budget);
        public void UpdateBudget(Budget budget) => _db.Update(budget);
        public void DeleteBudget(Budget budget) => _db.Delete(budget);

        // Categories
        public List<Category> GetCategories() => _db.Table<Category>().Where(c => c.IsActive).ToList();
        public List<Category> GetCategoriesByType(TransactionType type) => 
            _db.Table<Category>().Where(c => c.IsActive && c.TransactionType == type).ToList();
        public void AddCategory(Category category) => _db.Insert(category);
        public void UpdateCategory(Category category) => _db.Update(category);
        public void DeleteCategory(Category category) => _db.Delete(category);

        // Recurring Transactions
        public List<RecurringTransaction> GetRecurringTransactions() => 
            _db.Table<RecurringTransaction>().Where(rt => rt.IsActive).ToList();
        public List<RecurringTransaction> GetDueRecurringTransactions() => 
            _db.Table<RecurringTransaction>().Where(rt => rt.IsActive).ToList()
                .Where(rt => rt.IsDue).ToList();
        public void AddRecurringTransaction(RecurringTransaction recurringTransaction) => _db.Insert(recurringTransaction);
        public void UpdateRecurringTransaction(RecurringTransaction recurringTransaction) => _db.Update(recurringTransaction);
        public void DeleteRecurringTransaction(RecurringTransaction recurringTransaction) => _db.Delete(recurringTransaction);

        // Legacy methods for backwards compatibility
        public List<Transaction> GetAll() => GetAllTransactions();
        public void Add(Transaction transaction) => AddTransaction(transaction);
        public void Delete(Transaction transaction) => DeleteTransaction(transaction);
        public void Update(Transaction transaction) => UpdateTransaction(transaction);
    }
}
