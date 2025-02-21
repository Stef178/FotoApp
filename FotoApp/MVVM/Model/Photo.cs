using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FotoApp.MVVM.Model
{
    public class Photo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string ImagePath { get; set; }
        [NotNull]
        public int UserId { get; set; }
        [Ignore]
        public User User { get; set; }
        [Ignore]
        public Assignment Assignment { get; set; }
        [Ignore]
        public ICollection<Comment> Comments { get; set; }
    }
}
