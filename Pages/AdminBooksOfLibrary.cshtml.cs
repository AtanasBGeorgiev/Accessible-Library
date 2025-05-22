using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace Library.Pages
{
    public class AdminBooksOfLibraryModel : PageModel
    {
        public static List<BookInformation> listBooks = new List<BookInformation>();
        public static BookInformation books = new BookInformation();
        public static List<BookInformation> listDates = new List<BookInformation>();
        public static string checkWish;
        private string _showOnlyAvaiableBooks;
        private string _category;

        public string errorMessage = "";
        public string specialMessage = "";
        public void OnGet()
        {
            listDates.Clear();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query1 = $"SELECT DateOfTaking from [dbo].[Book] WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable='НЕ';";
                    using (SqlCommand command1 = new SqlCommand(query1, connection))
                    {
                        using (SqlDataReader reader = command1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BookInformation bookDateOfTaking = new BookInformation();

                                bookDateOfTaking.DateOfTaking = reader.GetString(0);

                                TimeSpan days = DateTime.Now - Convert.ToDateTime(bookDateOfTaking.DateOfTaking);

                                listDates.Add(bookDateOfTaking);
                            }
                        }
                    }

                    int counterBooks = 0;
                    TimeSpan ts;
                    for (int i = 0; i < listDates.Count; i++)
                    {
                        ts = DateTime.Now - Convert.ToDateTime(listDates[i].DateOfTaking);
                        if (ts.TotalDays > 30)
                        {
                            counterBooks++;
                        }
                    }

                    if (counterBooks > 0)
                    {
                        specialMessage = $"Има {counterBooks} книги, които са взети преди повече от 30 дни!";
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
            books.IsAvaiable = Request.Form["isAvaiable"];
            checkWish = Request.Form["days"];
            _showOnlyAvaiableBooks = Request.Form["showOnlyAvaiable"];
            _category = Request.Form["category"];

            try
            {
                for (int i = listBooks.Count - 1; i >= 0; i--)
                {
                    listBooks.Remove(listBooks[i]);
                }

                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "";
                    if (_category.Length > 0)
                    {
                        if (_showOnlyAvaiableBooks == "ДА")
                        {
                            sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={AdminLibraryInfoModel.libraryInfo.Id} and IsAvaiable='ДА' and Deduction='0' and Category='{_category}';";
                        }
                        else
                        {
                            if (books.IsAvaiable == "НЕ")
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={AdminLibraryInfoModel.libraryInfo.Id} and IsAvaiable=@isAvaiable and Deduction='0' and Category='{_category}';";
                            else
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={AdminLibraryInfoModel.libraryInfo.Id} and Deduction='0' and Category='{_category}';";
                        }
                    }
                    else
                    {
                        if (_showOnlyAvaiableBooks == "ДА")
                        {
                            sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={AdminLibraryInfoModel.libraryInfo.Id} and IsAvaiable='ДА' and Deduction='0';";
                        }
                        else
                        {
                            if (books.IsAvaiable == "НЕ")
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={AdminLibraryInfoModel.libraryInfo.Id} and IsAvaiable=@isAvaiable and Deduction='0';";
                            else
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={AdminLibraryInfoModel.libraryInfo.Id} and Deduction='0';";
                        }
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@isAvaiable", books.IsAvaiable);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BookInformation book = new BookInformation();
                                int id, year;
                                DateTime date;

                                id = reader.GetInt32(0);
                                book.Id = id.ToString();
                                book.InventoryNum = reader.GetString(1);
                                book.Title = reader.GetString(2);
                                book.Author = reader.GetString(3);
                                year = reader.GetInt32(4);
                                book.Year = year.ToString();
                                book.IsAvaiable = reader.GetString(5);
                                book.DateOfTaking = reader.GetString(6);
                                date = reader.GetDateTime(7);
                                book.DateOfCreation = date.ToString();

                                if (_showOnlyAvaiableBooks == "ДА" && book.IsAvaiable == "ДА")
                                {
                                    listBooks.Add(book);
                                }
                                else
                                {
                                    if (checkWish == "ДА")
                                    {
                                        if (book.IsAvaiable == "НЕ")
                                        {
                                            TimeSpan days = DateTime.Now - Convert.ToDateTime(book.DateOfTaking);

                                            if (days.Days > 30)
                                            {
                                                listBooks.Add(book);
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        listBooks.Add(book);
                                    }
                                }

                            }
                        }
                    }
                    connection.Close();
                }
                if (listBooks.Count == 0)
                {
                    errorMessage = "Не са открити резултати,отговарящи на търсенето.";
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
