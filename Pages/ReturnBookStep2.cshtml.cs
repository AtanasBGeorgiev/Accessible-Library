using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace Library.Pages
{
    public class ReturnBookStep2Model : PageModel
    {
        public static LibraryInformation libraryInfo = new LibraryInformation();
        public static BookInformation bookInfo = new BookInformation();

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            bookInfo.Signature = Request.Form["signature"];

            if (bookInfo.Signature.Trim().Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
            }
            else
            {
                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = $" UPDATE [dbo].[Book] SET Signature=@signature,IsAvaiable = 'ДА',IDReader= NULL,DateOfTaking='0' WHERE (ID = {ReturnBookStep1Model.bookInfo.Id});";
                        SqlCommand command1 = new SqlCommand(sql, connection);
                        command1.CommandType = CommandType.Text;
                        command1.Parameters.AddWithValue("@signature", bookInfo.Signature.Trim());
                        command1.ExecuteNonQuery();

                        ReturnBookStep1Model.bookInfo.Signature = bookInfo.Signature.Trim();

                        successMessage = "Върна книгата!";

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }
            }
        }
    }
}
