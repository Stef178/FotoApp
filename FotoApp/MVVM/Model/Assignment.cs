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

        [NotNull]
        public int DeadlineInMinutes { get; set; } // Aangepast van DateTime naar int

        [NotNull]
        public bool IsCompleted { get; set; } = false;

        [NotNull]
        public int ThemeId { get; set; }

        [Ignore]
        public AssignmentTheme Theme { get; set; }
    }

    public class AssignmentTheme
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    }
}
