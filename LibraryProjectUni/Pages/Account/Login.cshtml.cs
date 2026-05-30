using BusinessLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryProjectUni.Pages.Account
{
    public class LoginModel : ClsBaseController
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Error { get; set; }

        public IActionResult OnGet()
        {
            if (IsLoggedIn)
                return RedirectToDashboard();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Error = "Please enter email and password.";
                return Page();
            }

            ClsUser user = ClsUser.Login(Email, Password);

            if (user == null)
            {
                Error = "Invalid email or password.";
                return Page();
            }

            // Set session
            HttpContext.Session.SetInt32("UserID", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetInt32("RoleID", user.RoleId);

            return RedirectToDashboard();
        }

        private IActionResult RedirectToDashboard()
        {
            if (IsLibrarian)
                return RedirectToPage("/Librarian/Dashboard");

            return RedirectToPage("/Member/Dashboard");
        }
    }
}