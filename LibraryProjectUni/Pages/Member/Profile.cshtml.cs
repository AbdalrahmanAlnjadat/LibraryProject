using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryProjectUni.Pages.Member
{
    public class ProfileModel : ClsBaseController
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Error { get; set; } = "";
        public string Success { get; set; } = "";

        public IActionResult OnGet()
        {
            var check = RequireMember();
            if (check != null) return check;

            // Load current user data
            ClsUser? user = ClsUser.FindByUserId(SessionUserId);
            if (user != null)
            {
                FullName = user.FullName ?? "";
                Email = user.Email ?? "";
            }
            else
            {
                FullName = "";
                Email = "";
            }

            return Page();
        }

        public IActionResult OnPost(string? fullName, string? email)
        {
            var check = RequireMember();
            if (check != null) return check;

            // Accept both FullName and fullName (case insensitive)
            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = FullName;
            }

            // Accept both Email and email
            if (string.IsNullOrWhiteSpace(email))
            {
                email = Email;
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                Error = "Full name is required.";
                return Page();
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                Error = "Email is required.";
                return Page();
            }

            ClsUser? user = ClsUser.FindByUserId(SessionUserId);
            if (user == null)
            {
                Error = "User not found.";
                return Page();
            }

            user.FullName = fullName.Trim();
            user.Email = email.Trim();

            try
            {
                if (user.Save())
                {
                    Success = "Profile updated successfully!";
                    HttpContext.Session.SetString("FullName", user.FullName);
                    // Update local variables for display
                    FullName = user.FullName;
                    Email = user.Email;
                }
                else
                {
                    Error = "Failed to update profile.";
                }
            }
            catch (System.Exception ex)
            {
                Error = ex.Message;
            }

            return Page();
        }
    }
}
