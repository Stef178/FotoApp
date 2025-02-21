using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FotoApp.MVVM.Model
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AssignmentTheme Theme { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
    public class AssignmentTheme
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Assignment> Assignments { get; set; }
    }
}
