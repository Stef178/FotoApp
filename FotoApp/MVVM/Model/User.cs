using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FotoApp.MVVM.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Points { get; set; } = 5;
        public bool IsSuperMember { get; set; } = false;
        public UserRole Role { get; set; } = UserRole.Member;
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
    public enum UserRole
    {
        Member,
        SuperMember,
    }
}
