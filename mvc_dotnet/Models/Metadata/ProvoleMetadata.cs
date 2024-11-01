﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_dotnet.Models.Metadata
{
    public class ProvoleMetadata
    {
        [Display(Name = "Cinemas")]
        public int Cinemas { get; set; }

        [Display(Name = "Movies")]
        public int Movies { get; set; }

        [Display(Name = "Movies Name")]
        public string MoviesName { get; set; } = null!;

        [Display(Name = "Content Admin")]
        public int ContentAdmin { get; set; }

        [Display(Name = "Date & time of the Screening")]
        public DateTime? DatetimeColumn { get; set; }

    }
}
