using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FotoApp.MVVM.Model
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Username { get; set; }
        [NotNull]
        public string Email { get; set; }
        [NotNull]
        public string Password { get; set; }
        [NotNull]
        public int Points { get; set; } = 5;
        [NotNull]
        public bool IsActive { get; set; }
        [NotNull]
        public bool IsSuperMember { get; set; } = false;
        [NotNull]
        public UserRole Role { get; set; } = UserRole.Member;
        [Ignore]
        public ICollection<Photo> Photos { get; set; }
        [Ignore]
        public ICollection<Comment> Comments { get; set; }
    }
    public enum UserRole
    {
        Member,
        SuperMember,
    }
}
