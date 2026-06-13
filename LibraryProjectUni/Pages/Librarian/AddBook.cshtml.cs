using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProjectUni.Pages.Librarian
{
    public class AddBookModel : ClsBaseController
    {
        [BindProperty] public string Title { get; set; } = "";
        [BindProperty] public string Author { get; set; } = "";
        [BindProperty] public string Category { get; set; } = "";
        [BindProperty] public int Quantity { get; set; }
        [BindProperty] public decimal Price { get; set; }
        [BindProperty] public decimal DailyLateFee { get; set; }
        [BindProperty] public decimal PurchasePrice { get; set; }

        public string Error { get; set; } = "";

        public IActionResult OnGet()
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            Price = 1.00m;
            DailyLateFee = 1.00m;
            PurchasePrice = 10.00m;
            Quantity = 1;

            return Page();
        }

        public IActionResult OnPost()
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            if (string.IsNullOrWhiteSpace(Title))
            { Error = "Title is required."; return Page(); }

            if (string.IsNullOrWhiteSpace(Author))
            { Error = "Author is required."; return Page(); }

            if (string.IsNullOrWhiteSpace(Category))
            { Error = "Category is required."; return Page(); }

            if (Quantity <= 0)
            { Error = "Quantity must be at least 1."; return Page(); }

            if (Price < 0)
            { Error = "Rent fee cannot be negative."; return Page(); }

            if (DailyLateFee < 0)
            { Error = "Daily late fee cannot be negative."; return Page(); }

            if (PurchasePrice < 0)
            { Error = "Purchase price cannot be negative."; return Page(); }

            try
            {
                ClsBook book = new ClsBook();
                book.Title = Title.Trim();
                book.Author = Author.Trim();
                book.Category = Category;
                book.Quantity = Quantity;
                book.Price = Price;
                book.DailyLateFee = DailyLateFee;
                book.PurchasePrice = PurchasePrice;

                if (book.Save())
                    return RedirectToPage("/Librarian/Books");

                Error = "Failed to add book. Please try again.";
            }
            catch (System.Exception ex)
            {
                Error = ex.Message;
            }

            return Page();
        }
    }
}
