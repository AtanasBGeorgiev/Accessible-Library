using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Library.Pages
{
    public class AdminDeleteReaderModel : PageModel
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

                    int existRow;
                    string query1 = $"SELECT count(*) from [dbo].[Book] where IDReader={ReaderInfoForAdminModel.readerInfo.Id}";
                    
                    using (SqlCommand command1 = new SqlCommand(query1, connection))
                        existRow = (int)command1.ExecuteScalar();

                    if (existRow > 0) 
                    {
                        errorMessage = "Има книги,които не са върнати.";
                        return;
                    }
                    else
                    {
                        string query2 = $"DELETE FROM [dbo].[Reader] where ID={ReaderInfoForAdminModel.readerInfo.Id}";
                        
                        using (SqlCommand command2 = new SqlCommand(query2, connection))
                        {
                            command2.ExecuteNonQuery();
                        }

                        successMessage = "Изтри профила на читател и информацията,свързана с него.";
                    }
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
