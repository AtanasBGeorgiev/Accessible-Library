using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class AdminDeleteLibraryModel : PageModel
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
                string connectionsString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionsString))
                {
                    connection.Open();

                    int count;
                    string query1 = $"SELECT count(*) from [dbo].[Book] where IDLibrary={AdminLibraryInfoModel.libraryInfo.Id} and IsAvaiable=@isAvaiable;";
                    using (SqlCommand command1 = new SqlCommand(query1, connection))
                    {
                        command1.Parameters.AddWithValue("@isAvaiable", "НЕ");
                        count = (int)command1.ExecuteScalar();
                    }

                    if (count > 0)
                    {
                        errorMessage = "Има книги,които не са в библиотеката!" +
                            "Трябва да бъдат върнати и тогава ще можеш да изтриеш профила.";
                    }
                    else
                    {
                        string query2 = $"DELETE FROM [dbo].[Book] where IDLibrary={AdminLibraryInfoModel.libraryInfo.Id}";
                        using (SqlCommand command2 = new SqlCommand(query2, connection))
                        {
                            command2.ExecuteNonQuery();
                        }

                        string query3 = $"DELETE FROM [dbo].[Library] where ID={AdminLibraryInfoModel.libraryInfo.Id}";
                        using (SqlCommand command3 = new SqlCommand(query3, connection))
                            command3.ExecuteNonQuery();

                        successMessage = "Библиотеката и информацията,свързана с нея, са изтрити.";
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
