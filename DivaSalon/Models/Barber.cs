using System.ComponentModel.DataAnnotations;

namespace DivaSalon.Models
{
    public class Barber
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nume complet")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Specializare")]
        public string Specialization { get; set; } = string.Empty;

        [Display(Name = "Descriere")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "URL Poză")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Experiență (ani)")]
        public int YearsExperience { get; set; }

        [Display(Name = "Rating")]
        public double Rating { get; set; }

        [Display(Name = "Activ")]
        public bool IsActive { get; set; } = true;
    }
}