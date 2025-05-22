using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class ContactsModel : PageModel
    {
        public static List<AdminInformation> listAdmins = new List<AdminInformation>();

        public string errorMessage = "";
        public void OnGet()
        {
            listAdmins.Clear();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Email,Telephone from [dbo].[Administrator] where IsConfirmed='True';";

                    using (SqlCommand command1 = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AdminInformation admin = new AdminInformation();

                                admin.Email = reader.GetString(0);
                                admin.Telephone = reader.GetString(1);

                                listAdmins.Add(admin);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
