using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;

namespace Library.Pages
{
    public class CheckForBookModel : PageModel
    {
        public static BookInformation bookInfo = new BookInformation();
        public static List<BookInformation> listBooks = new List<BookInformation>();

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            bookInfo.Title = Request.Form["title"];
            bookInfo.Author = Request.Form["author"];

            if (bookInfo.Title.Length == 0 || bookInfo.Author.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
            }
            else
            {
                if (OftenUsedMethods.CorrectTopic(bookInfo) == "Грешка")
                {
                    errorMessage = "Заглавието на книгата трябва да е в кавички!";
                    return;
                }
                
                if (OftenUsedMethods.CorrectNameAndFamilia(bookInfo.Author) == "Едно име.")
                {
                    errorMessage = "Не може авторът да има само едно име!";
                    return;
                }
                
                try
                {
                    listBooks.Clear();

                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = $"SELECT ID,InventoryNum,Title,Author,Year,Signature FROM [dbo].[Book] WHERE Title=@title and Author=@author and IsAvaiable='ДА' and IDLibrary={LoginLibraryModel.libraryInfo.Id}";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@title", bookInfo.Title);
                            command.Parameters.AddWithValue("@author", bookInfo.Author);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    BookInformation book = new BookInformation();
                                    int id, year;
                                    id = reader.GetInt32(0);
                                    book.Id = id.ToString();
                                    book.InventoryNum = reader.GetString(1);
                                    book.Title = reader.GetString(2);
                                    book.Author = reader.GetString(3);
                                    year = reader.GetInt32(4);
                                    book.Year = year.ToString();
                                    book.Signature = reader.GetString(5);
                                   
                                    listBooks.Add(book);
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
                if (listBooks.Count > 0)
                {
                    successMessage = "Успешна проверка!";
                }
                else
                {
                    errorMessage = "Не бяха открити налични книги!";
                }
            }
        }
    }
}