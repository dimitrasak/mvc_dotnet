using System.ComponentModel.DataAnnotations;

namespace mvc_dotnet.Models.Metadata
{
    public class CustomerMetadata
    {
        [Display(Name = "User Username")]
        public string? UserUsername { get; set; }

        [Display(Name = "User Username")]
        public virtual User? UserUsernameNavigation { get; set; }

    }
}
