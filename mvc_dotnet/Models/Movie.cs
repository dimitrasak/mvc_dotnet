using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc_dotnet.Models
{
    [Table("movies")]
    public partial class Movie
    {
        public Movie()
        {
            Provoles = new HashSet<Provole>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Key]
        [Column("NAME")]
        [StringLength(45)]
        [Unicode(false)]
        public string Name { get; set; } = null!;
        [Column("CONTENT")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Content { get; set; }
        [Column("LENGTH")]
        public int? Length { get; set; }
        [Column("TYPE")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Type { get; set; }
        [Column("SUMMARY")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Summary { get; set; }
        [Column("DIRECTOR")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Director { get; set; }
        [Column("CONTENT_ADMIN_ID")]
        public int? ContentAdminId { get; set; }

        [ForeignKey("ContentAdminId")]
        [InverseProperty("Movies")]
        public virtual ContentAdmin? ContentAdmin { get; set; }
        [InverseProperty("Movies")]
        public virtual ICollection<Provole> Provoles { get; set; }
    }
}
