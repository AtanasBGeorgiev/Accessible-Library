using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;

namespace Library.Pages
{
    public class ReturnBookStep1Model : PageModel
    {
        public static LibraryInformation libraryInfo = new LibraryInformation();
        public static BookInformation bookInfo = new BookInformation();

        public string errorMessage = "";

        public void OnGet()
        {

        }
        public void OnPost()
        {
            bookInfo.InventoryNum = Request.Form["inventoryNum"];

            if (bookInfo.InventoryNum.Trim().Length == 0)
            {
                errorMessage = "Полето е задължително!";
            }
            else
            {
                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        bool exists = false;

                        string sql = $"SELECT count(*) FROM [dbo].[Book] where InventoryNum=@inventoryNum and IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable='НЕ';";
                        SqlCommand command = new SqlCommand(sql, connection);
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@inventoryNum", bookInfo.InventoryNum.Trim());

                        exists = (int)command.ExecuteScalar() > 0;

                        if (exists == true)
                        {
                            string sql2 = $"SELECT ID FROM [dbo].[Book] where InventoryNum=@inventoryNum and IDLibrary={LoginLibraryModel.libraryInfo.Id}";
                            SqlCommand command3 = new SqlCommand(sql2, connection);
                            command3.CommandType = CommandType.Text;
                            command3.Parameters.AddWithValue("@inventoryNum", bookInfo.InventoryNum.Trim());

                            bookInfo.Id = command3.ExecuteScalar().ToString();

                            string sql1 = $"SELECT Title,Author,Year,Signature FROM [dbo].[Book] where ID={bookInfo.Id};";
                            using (SqlCommand command1 = new SqlCommand(sql1, connection)) 
                            {
                                using (SqlDataReader reader = command1.ExecuteReader()) 
                                {
                                    while (reader.Read()) 
                                    {
                                        int year;
                                        bookInfo.Title = reader.GetString(0);
                                        bookInfo.Author = reader.GetString(1);
                                        year = reader.GetInt32(2);
                                        bookInfo.Year = year.ToString();
                                        bookInfo.Signature = reader.GetString(3);
                                    }
                                }
                            }

                            Response.Redirect("/ReturnBookStep2");
                        }
                        else
                        {
                            errorMessage = "Проверката не е успешна.";
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }

                bookInfo.InventoryNum = "";
            }
        }
    }
}