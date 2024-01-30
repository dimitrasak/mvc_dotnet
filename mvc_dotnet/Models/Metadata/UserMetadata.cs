using System.ComponentModel.DataAnnotations;

namespace mvc_dotnet.Models.Metadata
{
    public class UserMetadata
    {
        [Display(Name = "Create Time")]
        public DateTime? CreateTime { get; set; }
    }
}
