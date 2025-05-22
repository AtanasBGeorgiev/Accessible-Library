using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Pages
{
    public class LogoutLibraryModel : PageModel
    {
        public void OnGet()
        {
            LoginLibraryModel.libraryInfo.Id = "0";
            HttpContext.Session.Clear();
            Response.Redirect("/Index");
        }
    }
}