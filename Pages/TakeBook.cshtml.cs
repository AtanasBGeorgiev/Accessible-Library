using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;

namespace Library.Pages
{
    public class TakeBookModel : PageModel
    {
        public static  BookInformation bookInfo = new BookInformation();
        Reader reader = new Reader();

        public string successMessage = "";
        public string errorMessage = "";
        public DateTime date = DateTime.Now;
        public string idReader;
        public static string idBook;

        public void OnGet()
        {
            string id = Request.Query["id"];
            idBook = id;

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT ID,InventoryNum,Title,Author,Year,Signature FROM [dbo].[Book] WHERE ID=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id, year;
                                Id = reader.GetInt32(0);
                                bookInfo.InventoryNum = reader.GetString(1);
                                bookInfo.Title = reader.GetString(2);
                                bookInfo.Author = reader.GetString(3);
                                year = reader.GetInt32(4);
                                bookInfo.Year = year.ToString();
                                bookInfo.Signature = reader.GetString(5);
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
        public void OnPost()
        {
            bookInfo.Id = Request.Form["id"];
            reader.Email = Request.Form["email"];
            
            if (reader.Email.Length == 0)
            {
                errorMessage = "Полето Имейл е задължително!";
            }
            else
            {
                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql1 = "SELECT count(*) From [dbo].[Reader] WHERE Email=@email;";
                        bool exist = false;

                        using (SqlCommand command1 = new SqlCommand(sql1, connection))
                        {
                            command1.Parameters.AddWithValue("@email", reader.Email.Trim());

                            command1.ExecuteNonQuery();
                            exist = (int)command1.ExecuteScalar() > 0;
                        }
                        if (exist == true)
                        {
                            string sql2 = "SELECT ID FROM [dbo].[Reader] WHERE Email=@email;";

                            using (SqlCommand command2 = new SqlCommand(sql2, connection))
                            {
                                command2.Parameters.AddWithValue("@email", reader.Email.Trim());

                                command2.ExecuteNonQuery();
                                idReader = command2.ExecuteScalar().ToString();
                            }

                            string sql = $" UPDATE [dbo].[Book] SET IsAvaiable = 'НЕ',IDReader={idReader},DateOfTaking=@date WHERE ID = @idBook";

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@idBook", idBook);
                                bookInfo.DateOfTaking = date.ToString();
                                command.Parameters.AddWithValue("@date", bookInfo.DateOfTaking);

                                command.ExecuteNonQuery();
                            }

                            successMessage = "Успешно взе книгата.";
                        }
                        else
                        {
                            errorMessage = "Този имейл е грешен!";
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
}
