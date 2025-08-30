using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library_Management.Models
{
    // ViewModel for displaying author in lists
    public class AuthorListViewModel
    {
        public Guid AuthorId { get; set; }
        
        [Display(Name = "Author Name")]
        public string? Name { get; set; }
        
        public string? Biography { get; set; }
        
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }
        
        [Display(Name = "Profile Image")]
        public string? ProfileImageUrl { get; set; }
        
        [Display(Name = "Books Count")]
        public int BooksCount { get; set; }
        
        // Archive functionality
        public bool IsArchived { get; set; }
    }

    // ViewModel for adding new author
    public class AddAuthorViewModel
    {
        [Required(ErrorMessage = "Author name is required.")]
        [Display(Name = "Author Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Display(Name = "Biography")]
        [StringLength(1000, ErrorMessage = "Biography cannot exceed 1000 characters.")]
        public string? Biography { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Profile Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ProfileImageUrl { get; set; }
    }

    // ViewModel for editing author
    public class EditAuthorViewModel
    {
        [Required]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "Author name is required.")]
        [Display(Name = "Author Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Display(Name = "Biography")]
        [StringLength(1000, ErrorMessage = "Biography cannot exceed 1000 characters.")]
        public string? Biography { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Profile Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ProfileImageUrl { get; set; }
    }

    // ViewModel for author details page
    public class AuthorDetailsViewModel
    {
        public Guid AuthorId { get; set; }
        public string? Name { get; set; }
        public string? Biography { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsArchived { get; set; }
        
        // List of books by this author
        public List<BookListViewModel> Books { get; set; } = new();
        
        // Statistics
        public int TotalBooks { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
    }

    // ViewModel for pullout functionality (Part 2)
    public class PulloutBookCopyViewModel
    {
        [Required]
        public Guid BookCopyId { get; set; }
        
        public Guid BookId { get; set; }
        
        public string BookTitle { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Pullout reason is required.")]
        [Display(Name = "Pullout Reason")]
        public string PulloutReason { get; set; } = string.Empty;
        
        [Display(Name = "Additional Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }

    // ViewModel for displaying book copy details with pullout status
    public class BookCopyDetailsViewModel
    {
        public Guid CopyId { get; set; }
        public Guid BookId { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }
        public DateTime? PulloutDate { get; set; }
        public string? PulloutReason { get; set; }
        public bool IsAvailable { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
