using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Library.Pages
{
    public class AdminDeleteBookModel : PageModel
    {
        public static BookInformation bookInfo = new BookInformation();

        public string successMessage = "";
        public string errorMessage = "";
        private int _id;

        public void OnGet()
        {
            string id = Request.Query["id"];
            _id = int.Parse(id);

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Book WHERE ID=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id, year;
                                double price;

                                Id = reader.GetInt32(0);
                                bookInfo.Id = Id.ToString();
                                bookInfo.InventoryNum = reader.GetString(1);
                                bookInfo.Title = reader.GetString(2);
                                bookInfo.Author = reader.GetString(3);
                                year = reader.GetInt32(4);
                                bookInfo.Year = year.ToString();
                                price = reader.GetDouble(5);
                                bookInfo.Price = price.ToString();
                                bookInfo.Signature = reader.GetString(6);
                                bookInfo.IsAvaiable = reader.GetString(8);
                                bookInfo.Inventory = reader.GetString(11);
                                bookInfo.Category = reader.GetString(15);
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

            bookInfo.Deduction = Request.Form["removal"];

            if (bookInfo.Deduction.Length == 0 || bookInfo.Deduction == "0")
            {
                errorMessage = $"Полето ОТЧИСЛЯВАНЕ е задължително и не трябва да бъде {"0"}.";
            }
            else
            {
                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        if (bookInfo.IsAvaiable == "ДА") 
                        {
                            string query = $"UPDATE [dbo].[Book] SET Deduction=@deduction where ID={bookInfo.Id}";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@deduction", bookInfo.Deduction);
                                command.ExecuteNonQuery();
                            }
                            successMessage = "Отчисли тази книга.";
                        }
                        else
                        {
                            errorMessage = "Книгата трябва да се върне в библиотеката.";
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