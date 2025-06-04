using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceTracker.Models
{
    public class TransactionManager
    {
        public ObservableCollection<Transaction> Transactions { get; set; }
    }
}
