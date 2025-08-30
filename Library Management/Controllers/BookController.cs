using Library_Management.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            var books = BookService.Instance.GetBooks();
            return View(books);
        }

        public IActionResult AddModal()
        {
            return PartialView("_AddBookPartial");
        }

        [HttpPost]
        public IActionResult Add(AddBookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookService.Instance.AddBook(vm);
            return Ok();
        }

        public IActionResult EditModal(Guid id)
        {
            var editBookViewModel = BookService.Instance.GetBookById(id);
            if (editBookViewModel == null)
                return NotFound();

            return PartialView("_EditBookPartial", editBookViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditBookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookService.Instance.UpdateBook(vm);
            return Ok();
        }

        // ✅ UPDATED DeleteModal and Delete
        public IActionResult DeleteModal(Guid id)
        {
            var book = BookService.Instance.GetBookById(id);
            if (book == null)
                return NotFound();

            return PartialView("_DeleteBookPartial", book); // ✅ updated partial name
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var book = BookService.Instance.GetBookById(id);
            if (book == null)
                return NotFound();

            BookService.Instance.DeleteBook(id);
            return Ok(); // You can return a redirect if not using AJAX
        }

        public IActionResult Details(Guid id)
        {
            var book = BookService.Instance.GetBooks().FirstOrDefault(b => b.BookId == id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpPost]
        public IActionResult AddCopy(Guid id)
        {
            try
            {
                BookService.Instance.AddCopy(id);
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // New actions for Book Copy management
        public IActionResult AddCopyModal(Guid id)
        {
            var book = BookService.Instance.GetBooks().FirstOrDefault(b => b.BookId == id);
            if (book == null)
                return NotFound();

            var model = new AddBookCopyViewModel
            {
                BookId = id,
                BookTitle = book.Title ?? "Unknown Title",
                CoverImageUrl = book.CoverImageUrl,
                Condition = "New",
                Source = "Purchase"
            };

            return PartialView("_AddBookCopyPartial", model);
        }

        [HttpPost]
        public IActionResult AddBookCopy(AddBookCopyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                BookService.Instance.AddBookCopy(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Pullout Functionality - Part 2

        /// <summary>
        /// Display Pullout Book Copy modal
        /// </summary>
        /// <param name="copyId">Book Copy ID</param>
        /// <returns>Pullout modal partial view</returns>
        public IActionResult PulloutModal(Guid copyId)
        {
            // Get book copy details to populate the modal
            var bookCopies = BookService.Instance.GetBookCopiesDetails(Guid.Empty);
            var bookCopy = bookCopies.FirstOrDefault(bc => bc.CopyId == copyId);
            
            if (bookCopy == null)
                return NotFound();

            var book = BookService.Instance.GetBooks().FirstOrDefault(b => b.BookId == bookCopy.BookId);
            if (book == null)
                return NotFound();

            var model = new PulloutBookCopyViewModel
            {
                BookCopyId = copyId,
                BookId = bookCopy.BookId,
                BookTitle = book.Title ?? "Unknown Title",
                PulloutReason = "Damaged" // Default reason
            };

            return PartialView("_PulloutBookCopyPartial", model);
        }

        /// <summary>
        /// Process Pullout Book Copy request
        /// </summary>
        /// <param name="model">PulloutBookCopyViewModel</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        public IActionResult PulloutBookCopy(PulloutBookCopyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = BookService.Instance.PulloutBookCopy(model);
                if (!success)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

    }
}
