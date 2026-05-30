using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryProjectUni.Pages.Member
{
    public class BookDetailsModel : ClsBaseController
    {
        public ClsBook Book { get; set; }
        public string Message { get; set; }

        public IActionResult OnGet(int id)
        {
            var check = RequireMember();
            if (check != null) return check;

            Book = ClsBook.FindByBookId(id);

            if (Book == null)
                return RedirectToPage("/Member/Books");

            return Page();
        }

        public IActionResult OnPostBorrow(int bookId)
        {
            var check = RequireMember();
            if (check != null) return check;

            try
            {
                ClsBorrow borrow = new ClsBorrow();
                borrow.UserId = SessionUserId;
                borrow.BookId = bookId;
                borrow.Amount = ClsBorrow.DefaultFineAmount;

                if (borrow.Save())
                {
                    Message = "Book borrowed successfully!";
                    // Refresh book
                    Book = ClsBook.FindByBookId(bookId);
                }
            }
            catch (System.Exception ex)
            {
                Message = ex.Message;
            }

            if (Book != null)
                Book = ClsBook.FindByBookId(bookId);

            return Page();
        }

        public IActionResult OnPostBuyDirect(int bookId, decimal price)
        {
            var check = RequireMember();
            if (check != null) return check;

            try
            {
                if (ClsBook.BuyBookDirect(SessionUserId, bookId, price))
                {
                    Message = $"Book purchased for ${price}! It's yours now.";
                    // Refresh book
                    Book = ClsBook.FindByBookId(bookId);
                }
                else
                {
                    Message = "Failed to purchase book.";
                }
            }
            catch (System.Exception ex)
            {
                Message = ex.Message;
            }

            if (Book != null)
                Book = ClsBook.FindByBookId(bookId);

            return Page();
        }
    }
}