using System.ComponentModel.DataAnnotations;

namespace mvc_dotnet.Models.Metadata
{
    public class UserMetadata
    {
        [Display(Name = "Create Time")]
        public DateTime? CreateTime { get; set; }

        [Required(ErrorMessage = "Please enter a user name.")]
        public string Username { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter a password.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        public string? Email { get; set; }



    }
}
