using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc_dotnet.Models
{
    [Table("user")]
    public partial class User
    {
        public User()
        {
            Admins = new HashSet<Admin>();
            ContentAdmins = new HashSet<ContentAdmin>();
            Customers = new HashSet<Customer>();
        }

        [Key]
        [Column("username")]
        [StringLength(32)]
        [Unicode(false)]
        public string Username { get; set; } = null!;
        [Column("email")]
        [StringLength(32)]
        [Unicode(false)]
        public string? Email { get; set; }
        [Column("password")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Password { get; set; }
        [Column("create_time", TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column("salt")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Salt { get; set; }
        [Column("role")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Role { get; set; }

        [InverseProperty("UserUsernameNavigation")]
        public virtual ICollection<Admin> Admins { get; set; }
        [InverseProperty("UserUsernameNavigation")]
        public virtual ICollection<ContentAdmin> ContentAdmins { get; set; }
        [InverseProperty("UserUsernameNavigation")]
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
