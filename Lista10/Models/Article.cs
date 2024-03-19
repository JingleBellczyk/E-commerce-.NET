using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lista10.Models
{
    public class Article
    {

        public int Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "To short name")]
        [Display(Name = "Product name")]
        public string Name { get; set; }

        [Display(Name = "Product price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public double Price { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime ExpirationDate { get; set; }

        // [Required]
        public int? CategoryId { get; set; }

        //do nawigacji
        public Category? Category { get; set; }

        //nie bedzie do bazy danych mialo zadnego zwiazku
        [NotMapped]
        public IFormFile? formFile { get; set; }
        public String? imagePath { get; set; }

    }
}