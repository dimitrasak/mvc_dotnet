using System.ComponentModel.DataAnnotations;

namespace mvc_dotnet.Models.Metadata
{
    public class ReservationMetadata
    {
        [Display(Name = "Number Of Seats")]
        public int? NumberOfSeats { get; set; }

        [Display(Name = "Cinemas")]
        public int? ProvolesCinemasId { get; set; }

        [Display(Name = "Movies Id")]
        public int? ProvolesMoviesId { get; set; }

        [Display(Name = "Movies Name")]
        public string? ProvolesMoviesName { get; set; }

        [Display(Name = "Customer")]
        public int? CustomersId { get; set; }



    }
}
