using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryProjectUni.Pages.Member
{
    public class BookDetailsModel : ClsBaseController
    {
        public ClsBook Book { get; set; } = new();
        public string Message { get; set; } = "";

        public IActionResult OnGet(int id)
        {
            var check = RequireMember();
            if (check != null) return check;

            ClsBook? book = ClsBook.FindByBookId(id);

            if (book == null)
                return RedirectToPage("/Member/Books");

            Book = book;
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
                    RefreshBook(bookId);
                }
            }
            catch (System.Exception ex)
            {
                Message = ex.Message;
            }

            RefreshBook(bookId);

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
                    RefreshBook(bookId);
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

            RefreshBook(bookId);

            return Page();
        }

        private void RefreshBook(int bookId)
        {
            Book = ClsBook.FindByBookId(bookId) ?? Book;
        }
    }
}
