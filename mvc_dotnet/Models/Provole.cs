using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc_dotnet.Models
{
    [Table("provoles")]
    public partial class Provole
    {
        public Provole()
        {
            Reservations = new HashSet<Reservation>();
        }

        public Provole (CreateProvoleModel model)
        {
            this.Id = model.Id;
            this.CinemasId = model.CinemasId;
            this.MoviesId = model.MoviesId;
            this.MoviesName = model.MoviesName;
            this.ContentAdminId = model.ContentAdminId;
            this.DatetimeColumn = model.DatetimeColumn;
        }

        [Column("ID")]
        public int Id { get; set; }
        [Key]
        [Column("CINEMAS_ID")]
        public int CinemasId { get; set; }
        [Key]
        [Column("MOVIES_ID")]
        public int MoviesId { get; set; }
        [Key]
        [Column("MOVIES_NAME")]
        [StringLength(45)]
        [Unicode(false)]
        public string MoviesName { get; set; } = null!;
        [Column("CONTENT_ADMIN_ID")]
        public int ContentAdminId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DatetimeColumn { get; set; }

        [ForeignKey("CinemasId")]
        [InverseProperty("Provoles")]
        public virtual Cinema Cinemas { get; set; } = null!;
        [ForeignKey("ContentAdminId")]
        [InverseProperty("Provoles")]
        public virtual ContentAdmin ContentAdmin { get; set; } = null!;
        [ForeignKey("MoviesId,MoviesName")]
        [InverseProperty("Provoles")]
        public virtual Movie Movies { get; set; } = null!;
        [InverseProperty("Provoles")]
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
