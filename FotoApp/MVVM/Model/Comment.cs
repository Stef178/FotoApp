using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FotoApp.MVVM.Model
{
    public class Comment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Text { get; set; }
        [Ignore]
        public User User { get; set; }

        public int UserId { get; set; }

        public int PhotoId { get; set; }
        [Ignore]
        public Photo Photo { get; set; }
    }
}
