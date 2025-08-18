using System;
using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models
{
    public class AddBookCopyViewModel
    {
        [Required]
        public Guid BookId { get; set; }
        
        public string BookTitle { get; set; } = string.Empty;
        
        [Display(Name = "Cover Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? CoverImageUrl { get; set; }
        
        [Required(ErrorMessage = "Condition is required")]
        [Display(Name = "Condition")]
        public string Condition { get; set; } = "New";
        
        [Required(ErrorMessage = "Source is required")]
        [Display(Name = "Source")]
        public string Source { get; set; } = "Purchase";
    }
}
