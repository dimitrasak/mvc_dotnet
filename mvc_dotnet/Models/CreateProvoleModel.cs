using System.ComponentModel.DataAnnotations;

namespace mvc_dotnet.Models
{
    public partial class CreateProvoleModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int CinemasId { get; set; }
        [Required]
        public int MoviesId { get; set; }
        [Required]
        public string MoviesName { get; set; } = null!;
        [Required]
        public int ContentAdminId { get; set; }
        public DateTime? DatetimeColumn { get; set; }

    }
}
