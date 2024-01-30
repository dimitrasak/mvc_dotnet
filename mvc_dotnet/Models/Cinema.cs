using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc_dotnet.Models
{
    [Table("cinemas")]
    public partial class Cinema
    {
        public Cinema()
        {
            Provoles = new HashSet<Provole>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("NAME")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Name { get; set; }
        [Column("SEATS")]
        [StringLength(45)]
        [Unicode(false)]
        public string? Seats { get; set; }
        [Column("3D")]
        [StringLength(45)]
        [Unicode(false)]
        public string? _3d { get; set; }

        [InverseProperty("Cinemas")]
        public virtual ICollection<Provole> Provoles { get; set; }
    }
}
