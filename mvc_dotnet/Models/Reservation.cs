using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc_dotnet.Models
{
    [Table("reservations")]
    public partial class Reservation
    {
        [Column("NUMBER_OF_SEATS")]
        public int? NumberOfSeats { get; set; }
        [Column("PROVOLES_CINEMAS_ID")]
        public int? ProvolesCinemasId { get; set; }
        [Column("PROVOLES_MOVIES_ID")]
        public int? ProvolesMoviesId { get; set; }
        [Column("PROVOLES_MOVIES_NAME")]
        [StringLength(45)]
        [Unicode(false)]
        public string? ProvolesMoviesName { get; set; }
        [Column("CUSTOMERS_ID")]
        public int? CustomersId { get; set; }
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [ForeignKey("CustomersId")]
        [InverseProperty("Reservations")]
        public virtual Customer? Customers { get; set; }
        [ForeignKey("ProvolesCinemasId,ProvolesMoviesId,ProvolesMoviesName")]
        [InverseProperty("Reservations")]
        public virtual Provole? Provoles { get; set; }
    }
}
