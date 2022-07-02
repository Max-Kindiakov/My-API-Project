using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ParkyAPI.Models.Trail;

namespace ParkyAPI.Models.Dtos
{
    public class TrailUpdateDto  //для створення та редагування
    {
        
        public int Id { get; set; } //тут треба айді, бо за ним я буду змінювати тропинки
        [Required]
        public string Name { get; set; }
        [Required]
        public double Distance { get; set; }
        
        public DifficultyType Difficulty { get; set; }
        [Required]
        public int NationalParkId { get; set; }
        [Required]
        public double Elevation { get; set; }

        //видалив запрос на національні парки
    }
}

