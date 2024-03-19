using System.ComponentModel.DataAnnotations;

namespace Lista10.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "To short name")]
        public string Name { get; set; }

        public ICollection<Article> Articles { get; set; }  
    }
}
