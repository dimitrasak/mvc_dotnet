using System.ComponentModel.DataAnnotations;

namespace mvc_dotnet.Models.Metadata
{
    public class MovieMetadata
    {
        [Display(Name = "Content Admin Id")]
        public int? ContentAdminId { get; set; }

    }
}
