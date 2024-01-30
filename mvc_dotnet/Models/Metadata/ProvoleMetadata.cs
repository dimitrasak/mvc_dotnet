using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_dotnet.Models.Metadata
{
    public class ProvoleMetadata
    {
        [Display(Name = "Cinemas Id")]
        public int CinemasId { get; set; }

        [Display(Name = "Movies Id")]
        public int MoviesId { get; set; }

        [Display(Name = "Movies Name")]
        public string MoviesName { get; set; } = null!;

        [Display(Name = "Content Admin Id")]
        public int ContentAdminId { get; set; }

    }
}
