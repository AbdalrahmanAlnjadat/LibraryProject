using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProjectUni.Pages.Librarian
{
    public class EditBookModel : ClsBaseController
    {
        [BindProperty]
        public ClsBook Book { get; set; }

        public string Error { get; set; }
        public string Success { get; set; }

        public IActionResult OnGet(int id)
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            Book = ClsBook.FindByBookId(id);

            if (Book == null)
                return RedirectToPage("/Librarian/Books");

            return Page();
        }

        public IActionResult OnPost()
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            if (Book == null || Book.BookId <= 0)
            {
                Error = "Invalid book data.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Book.Title))
            { Error = "Title is required."; return Page(); }

            if (string.IsNullOrWhiteSpace(Book.Author))
            { Error = "Author is required."; return Page(); }

            if (string.IsNullOrWhiteSpace(Book.Category))
            { Error = "Category is required."; return Page(); }

            if (Book.Quantity < 0)
            { Error = "Quantity cannot be negative."; return Page(); }

            if (Book.Price < 0)
            { Error = "Rent fee cannot be negative."; return Page(); }

            if (Book.DailyLateFee < 0)
            { Error = "Daily late fee cannot be negative."; return Page(); }

            if (Book.PurchasePrice < 0)
            { Error = "Purchase price cannot be negative."; return Page(); }

            try
            {
                Book.Mode = ClsBook.enMode.Update;

                if (Book.Save())
                    return RedirectToPage("/Librarian/Books");

                Error = "Failed to update book.";
            }
            catch (System.Exception ex)
            {
                Error = ex.Message;
            }

            return Page();
        }
    }
}