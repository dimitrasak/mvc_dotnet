using System.ComponentModel.DataAnnotations;

namespace mvc_dotnet.Models.Metadata
{
    public class MovieMetadata
    {
        [Display(Name = "Content Admin")]
        public int? ContentAdmin { get; set; }

    }
}
