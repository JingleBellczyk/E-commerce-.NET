using Lista10.Models;
using System.ComponentModel.DataAnnotations;

namespace Lista10.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "To short name")]
        public string Name { get; set; }
    }
}
