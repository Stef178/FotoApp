using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FotoApp.MVVM.Model
{
    public class Assignment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Title { get; set; }
        [NotNull]
        public string Description { get; set; }
        [Ignore]
        public AssignmentTheme Theme { get; set; }
        [NotNull]
        public DateTime Deadline { get; set; }
        [NotNull]
        public bool IsCompleted { get; set; } = false;
    }
    public class AssignmentTheme
    {
        [NotNull]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
    }
}
