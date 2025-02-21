using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FotoApp.MVVM.Model
{
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int UserId { get; set; }
        [Ignore]
        public User User { get; set; }
        [NotNull]
        public int Points { get; set; }
        [NotNull]
        public decimal AmountPaid { get; set; }
        [NotNull]
        public DateTime Date { get; set; }
    }
}
