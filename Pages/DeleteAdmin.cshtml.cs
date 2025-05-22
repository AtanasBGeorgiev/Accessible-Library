using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class DeleteAdminModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query2 = $"DELETE FROM [dbo].[Administrator] where ID={AdminInfoModel.adminInfo.Id}";              
                    using (SqlCommand command2 = new SqlCommand(query2, connection))
                    {
                        command2.ExecuteNonQuery();
                    }

                    successMessage = "Изтри профила на администратор.";

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
