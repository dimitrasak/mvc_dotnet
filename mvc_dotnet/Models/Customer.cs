using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace mvc_dotnet.Models
{
    [Table("customers")]
    public partial class Customer
    {
        public Customer()
        {
            Reservations = new HashSet<Reservation>();
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
        [InverseProperty("Customers")]
        public virtual User? UserUsernameNavigation { get; set; }
        [InverseProperty("Customers")]
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
