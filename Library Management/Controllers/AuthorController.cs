using Library_Management.Models;
using Library_Management.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management.Controllers
{
    /// <summary>
    /// Controller for Author CRUD operations
    /// Part 1: Author Management - Complete CRUD functionality
    /// </summary>
    public class AuthorController : Controller
    {
        #region Author Index and Details

        /// <summary>
        /// Display list of all authors (excluding archived)
        /// </summary>
        /// <returns>Author Index view</returns>
        public IActionResult Index()
        {
            var authors = AuthorService.Instance.GetAuthors(includeArchived: false);
            return View(authors);
        }

        /// <summary>
        /// Display author details with their books
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Author Details view</returns>
        public IActionResult Details(Guid id)
        {
            var author = AuthorService.Instance.GetAuthorDetails(id);
            if (author == null)
                return NotFound();

            return View(author);
        }

        #endregion

        #region Author Create

        /// <summary>
        /// Display Add Author modal
        /// </summary>
        /// <returns>Add Author partial view</returns>
        public IActionResult AddModal()
        {
            var model = new AddAuthorViewModel();
            return PartialView("_AddAuthorPartial", model);
        }

        /// <summary>
        /// Process Add Author form submission
        /// </summary>
        /// <param name="model">AddAuthorViewModel</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        public IActionResult Add(AddAuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var authorId = AuthorService.Instance.AddAuthor(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Author Edit

        /// <summary>
        /// Display Edit Author modal
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Edit Author partial view</returns>
        public IActionResult EditModal(Guid id)
        {
            var author = AuthorService.Instance.GetAuthorForEdit(id);
            if (author == null)
                return NotFound();

            return PartialView("_EditAuthorPartial", author);
        }

        /// <summary>
        /// Process Edit Author form submission
        /// </summary>
        /// <param name="model">EditAuthorViewModel</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        public IActionResult Edit(EditAuthorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = AuthorService.Instance.UpdateAuthor(model);
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

        #region Author Delete

        /// <summary>
        /// Display Delete Author confirmation modal
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Delete Author partial view</returns>
        public IActionResult DeleteModal(Guid id)
        {
            var author = AuthorService.Instance.GetAuthorDetails(id);
            if (author == null)
                return NotFound();

            return PartialView("_DeleteAuthorPartial", author);
        }

        /// <summary>
        /// Process Delete Author request
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var success = AuthorService.Instance.DeleteAuthor(id);
                if (!success)
                    return NotFound();

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                // Author has books - cannot delete
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Archive Operations - Part 3

        /// <summary>
        /// Display archived authors
        /// </summary>
        /// <returns>Archived Authors view</returns>
        public IActionResult Archive()
        {
            var archivedAuthors = AuthorService.Instance.GetArchivedAuthors();
            return View(archivedAuthors);
        }

        /// <summary>
        /// Display Archive Author modal
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Archive Author partial view</returns>
        public IActionResult ArchiveModal(Guid id)
        {
            var author = AuthorService.Instance.GetAuthorDetails(id);
            if (author == null)
                return NotFound();

            return PartialView("_ArchiveAuthorPartial", author);
        }

        /// <summary>
        /// Process Archive Author request
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <param name="reason">Archive reason</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        public IActionResult ArchiveAuthor(Guid id, string reason)
        {
            try
            {
                var success = AuthorService.Instance.ArchiveAuthor(id, reason);
                if (!success)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Restore archived author
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>Success or error response</returns>
        [HttpPost]
        public IActionResult RestoreAuthor(Guid id)
        {
            try
            {
                var success = AuthorService.Instance.RestoreAuthor(id);
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
