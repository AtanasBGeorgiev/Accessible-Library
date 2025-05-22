using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System;

namespace Library.Pages
{
    public class LibraryBooksModel : PageModel
    {
        public static List<BookInformation> listBooks = new List<BookInformation>();
        public static BookInformation books = new BookInformation();
        public static string checkWish;
        private string _showOnlyAvaiableBooks;
        public string _category;

        public string errorMessage = "";
        public void OnGet()
        {
        }
        public void OnPost()
        {
            books.IsAvaiable = Request.Form["isAvaiable"];
            checkWish = Request.Form["days"];
            _showOnlyAvaiableBooks = Request.Form["showOnlyAvaiable"];
            _category = Request.Form["category"];

            try
            {
                listBooks.Clear();
               
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "";

                    if (_category.Length > 0) 
                    {
                        if (_showOnlyAvaiableBooks == "ДА")
                        {
                            sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable='ДА' and Deduction='0' and Category='{_category}';";
                        }
                        else
                        {
                            if (books.IsAvaiable == "НЕ")
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable=@isAvaiable and Deduction='0' and Category='{_category}';";
                            else
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and Deduction='0' and Category='{_category}';";
                        }
                    }
                    else
                    {
                        if (_showOnlyAvaiableBooks == "ДА")
                        {
                            sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable='ДА' and Deduction='0';";
                        }
                        else
                        {
                            if (books.IsAvaiable == "НЕ")
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable=@isAvaiable and Deduction='0';";
                            else
                                sql = $"SELECT ID,InventoryNum,Title,Author,Year,IsAvaiable,DateOfTaking,DateOfCreation FROM Book WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and Deduction='0';";
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