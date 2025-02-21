using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FotoApp.MVVM.Model
{
    public class Photo
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
