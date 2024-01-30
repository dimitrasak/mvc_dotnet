using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc_dotnet.Models
{
    [Table("content_admin")]
    public partial class ContentAdmin
    {
        public ContentAdmin()
        {
            Movies = new HashSet<Movie>();
            Provoles = new HashSet<Provole>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("NAME")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Name { get; set; }
        [Column("user_username")]
        [StringLength(32)]
        [Unicode(false)]
        public string? UserUsername { get; set; }

        [ForeignKey("UserUsername")]
        [InverseProperty("ContentAdmins")]
        public virtual User? UserUsernameNavigation { get; set; }
        [InverseProperty("ContentAdmin")]
        public virtual ICollection<Movie> Movies { get; set; }
        [InverseProperty("ContentAdmin")]
        public virtual ICollection<Provole> Provoles { get; set; }
    }
}
