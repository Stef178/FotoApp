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
        public string Countdown { get; set; }

        public bool IsAvailable { get; set; } = true; // Indicates if the assignment is currently available
        [NotNull]
        public int DeadlineInMinutes { get; set; } // Time limit for the assignment in minutes

        [NotNull]
        public bool IsCompleted { get; set; } = false;

        [NotNull]
        public int ThemeId { get; set; }

        [Ignore]
        public string ImagePath { get; set; }

        [Ignore]
        public AssignmentTheme Theme { get; set; }

        // Nieuwe eigenschappen voor timerfunctionaliteit
        public int RemainingTimeInMinutes { get; set; } = 0; // Resterende tijd voor de opdracht
        public bool IsTimerRunning { get; set; } = false; // Of de timer actief is
    }
}



public class AssignmentTheme
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    }
