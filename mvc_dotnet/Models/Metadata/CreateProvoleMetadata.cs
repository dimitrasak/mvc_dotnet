using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_dotnet.Models.Metadata
{
    public class CreateProvoleMetadata
    {
        [Display(Name = "Cinemas")]
        public int CinemasId { get; set; }

        [Display(Name = "Movies")]
        public int MoviesId { get; set; }

        [Display(Name = "Movies Name")]
        public string MoviesName { get; set; } = null!;

        [Display(Name = "Content Admin")]
        public int ContentAdminId { get; set; }

        [Display(Name = "Date & time of the Screening")]
        public DateTime? DatetimeColumn { get; set; }

    }
}
