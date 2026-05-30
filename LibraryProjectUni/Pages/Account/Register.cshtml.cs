using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryProjectUni.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty] public string FullName { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }

        public string Error { get; set; }

        public IActionResult OnGet() => Page();

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                Error = "All fields are required.";
                return Page();
            }

            try
            {
                ClsUser user = new ClsUser();
                user.FullName = FullName.Trim();
                user.Email = Email.Trim();
                user.Password = Password;

                if (user.Save())
                    return RedirectToPage("/Account/Login");

                Error = "Registration failed. Please try again.";
            }
            catch (System.InvalidOperationException ex)
            {
                Error = ex.Message; // "This email is already registered."
            }
            catch (System.Exception ex)
            {
                Error = ex.Message;
            }

            return Page();
        }
    }
}