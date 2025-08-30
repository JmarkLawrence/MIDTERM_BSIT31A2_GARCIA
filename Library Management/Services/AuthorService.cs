using Library_Management.Models;
using Library_Management_Domain.Entities;

namespace Library_Management.Services
{
    /// <summary>
    /// Service class for managing Author CRUD operations and business logic
    /// Part 1: Author Management - Complete CRUD functionality
    /// </summary>
    public class AuthorService
    {
        private readonly ICollection<Author> _authors;
        private readonly ICollection<Book> _books;
        private readonly ICollection<BookCopy> _bookCopies;

        // Singleton pattern
        private static AuthorService? _instance;
        public static AuthorService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthorService();
                }
                return _instance;
            }
        }

        private AuthorService()
        {
            // Get references to existing collections from BookService
            _authors = BookService.Instance.GetAuthorsCollection();
            _books = BookService.Instance.GetBooksCollection();
            _bookCopies = BookService.Instance.GetBookCopiesCollection();
        }

        #region Author CRUD Operations

        /// <summary>
        /// Get all authors (excluding archived ones by default)
        /// </summary>
        /// <param name="includeArchived">Include archived authors in results</param>
        /// <returns>List of AuthorListViewModel</returns>
        public IEnumerable<AuthorListViewModel> GetAuthors(bool includeArchived = false)
        {
            var authorsQuery = includeArchived ? _authors : _authors.Where(a => !a.IsArchived);

            return authorsQuery.Select(a => new AuthorListViewModel
            {
                AuthorId = a.Id,
                Name = a.Name,
                Biography = a.Biography,
                BirthDate = a.BirthDate,
                ProfileImageUrl = a.ProfileImageUrl,
                BooksCount = _books.Count(b => !b.IsArchived && a.Books.Any(ab => ab.Id == b.Id)),
                IsArchived = a.IsArchived
            }).OrderBy(a => a.Name);
        }

        /// <summary>
        /// Get author details by ID
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>AuthorDetailsViewModel or null if not found</returns>
        public AuthorDetailsViewModel? GetAuthorDetails(Guid id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return null;

            // Get books by this author (excluding archived books)
            var authorBooks = _books.Where(b => !b.IsArchived && author.Books.Any(ab => ab.Id == b.Id)).ToList();
            
            var bookViewModels = authorBooks.Select(b => new BookListViewModel
            {
                BookId = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                Description = b.Description,
                Genre = b.Genre,
                PublishedDate = b.PublishedDate,
                CoverImageUrl = _bookCopies.FirstOrDefault(bc => bc.Book?.Id == b.Id)?.CoverImageUrl,
                AuthorName = author.Name,
                AuthorProfileImageUrl = author.ProfileImageUrl,
                TotalCopies = _bookCopies.Count(bc => bc.Book?.Id == b.Id),
                AvailableCopies = _bookCopies.Count(bc => bc.Book?.Id == b.Id && bc.PulloutDate == null)
            }).ToList();

            return new AuthorDetailsViewModel
            {
                AuthorId = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BirthDate = author.BirthDate,
                ProfileImageUrl = author.ProfileImageUrl,
                IsArchived = author.IsArchived,
                Books = bookViewModels,
                TotalBooks = bookViewModels.Count,
                TotalCopies = bookViewModels.Sum(b => b.TotalCopies),
                AvailableCopies = bookViewModels.Sum(b => b.AvailableCopies)
            };
        }

        /// <summary>
        /// Get author for editing
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>EditAuthorViewModel or null if not found</returns>
        public EditAuthorViewModel? GetAuthorForEdit(Guid id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return null;

            return new EditAuthorViewModel
            {
                AuthorId = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BirthDate = author.BirthDate,
                ProfileImageUrl = author.ProfileImageUrl
            };
        }

        /// <summary>
        /// Add new author
        /// </summary>
        /// <param name="model">AddAuthorViewModel</param>
        /// <returns>Created author ID</returns>
        public Guid AddAuthor(AddAuthorViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            var newAuthor = new Author
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Biography = model.Biography,
                BirthDate = model.BirthDate,
                ProfileImageUrl = model.ProfileImageUrl,
                IsArchived = false, // New authors are not archived
                Books = new List<Book>()
            };

            _authors.Add(newAuthor);
            return newAuthor.Id;
        }

        /// <summary>
        /// Update existing author
        /// </summary>
        /// <param name="model">EditAuthorViewModel</param>
        /// <returns>True if successful, false if author not found</returns>
        public bool UpdateAuthor(EditAuthorViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            var author = _authors.FirstOrDefault(a => a.Id == model.AuthorId);
            if (author == null)
                return false;

            // Update author properties
            author.Name = model.Name;
            author.Biography = model.Biography;
            author.BirthDate = model.BirthDate;
            author.ProfileImageUrl = model.ProfileImageUrl;

            return true;
        }

        /// <summary>
        /// Delete author (considers books association)
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>True if successful, false if author not found or has books</returns>
        public bool DeleteAuthor(Guid id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return false;

            // Check if author has any books (including archived ones)
            if (author.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete author who has books. Please remove or reassign books first.");
            }

            _authors.Remove(author);
            return true;
        }

        #endregion

        #region Archive Operations - Part 3

        /// <summary>
        /// Archive an author (soft delete)
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <param name="reason">Archive reason</param>
        /// <returns>True if successful</returns>
        public bool ArchiveAuthor(Guid id, string reason)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return false;

            author.IsArchived = true;
            author.ArchivedDate = DateTime.Now;
            author.ArchiveReason = reason;

            return true;
        }

        /// <summary>
        /// Restore archived author
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>True if successful</returns>
        public bool RestoreAuthor(Guid id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return false;

            author.IsArchived = false;
            author.ArchivedDate = null;
            author.ArchiveReason = null;

            return true;
        }

        /// <summary>
        /// Get archived authors
        /// </summary>
        /// <returns>List of archived authors</returns>
        public IEnumerable<AuthorListViewModel> GetArchivedAuthors()
        {
            return GetAuthors(includeArchived: true).Where(a => a.IsArchived);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check if author exists
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>True if exists</returns>
        public bool AuthorExists(Guid id)
        {
            return _authors.Any(a => a.Id == id);
        }

        /// <summary>
        /// Get author name by ID
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Author name or "Unknown Author"</returns>
        public string GetAuthorName(Guid id)
        {
            return _authors.FirstOrDefault(a => a.Id == id)?.Name ?? "Unknown Author";
        }

        #endregion
    }
}
