using Cyrillic.Convert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Library.Pages
{
    public class BookSearchModel : PageModel
    {
        public static List<LibraryInformation> listLibraries = new List<LibraryInformation>();
        public static List<BookInformation> listBooks = new List<BookInformation>();
        public static List<string> addresses = new List<string>();

        BookInformation bookInfo = new BookInformation();
        LibraryInformation librariesInfo = new LibraryInformation();

        public string successMessage = "";
        public string errorMessage = "";

        string firstYear, lastYear;

        public void OnGet()
        {
        }

        public void OnPost()
        {
            ClearLists();

            bookInfo.Title = Request.Form["title"];
            bookInfo.Author = Request.Form["author"];
            bookInfo.IsAvaiable = Request.Form["isAvaiable"];
            bookInfo.Category = Request.Form["category"];

            firstYear = Request.Form["begin"];
            lastYear = Request.Form["end"];

            librariesInfo.District = Request.Form["district"];
            librariesInfo.Municipality = Request.Form["municipality"];
            librariesInfo.CityVillage = Request.Form["cityVillage"];

            if (bookInfo.Title.Length == 2 && bookInfo.Author.Length == 0 && bookInfo.Category.Length == 0 &&
                firstYear.Length == 0 && lastYear.Length == 0)
            {
                errorMessage = "Поне едно от първите пет полета трябва да е попълнено!";
            }
            else
            {
                if (bookInfo.Title.Length != 2)
                {
                    if (OftenUsedMethods.CorrectTopic(bookInfo) == "Грешка")
                    {
                        errorMessage = "Заглавието на книгата трябва да е в кавички!";
                        return;
                    }
                }
                else if (bookInfo.Author.Length != 0)
                {
                    if (OftenUsedMethods.CorrectNameAndFamilia(bookInfo.Author) == "Едно име.")
                    {
                        errorMessage = "Не може авторът да има само едно име!";
                        return;
                    }
                }
                else if (librariesInfo.CityVillage.Length > 0)
                {
                    librariesInfo.CityVillage = OftenUsedMethods.CorrectCityAndMunicipalityInput(librariesInfo.CityVillage);
                }
                else if (librariesInfo.Municipality.Length > 0)
                {
                    librariesInfo.Municipality = OftenUsedMethods.CorrectCityAndMunicipalityInput(librariesInfo.Municipality); ;
                }
                else if (firstYear.Length > 0)
                {
                    if (OftenUsedMethods.CorrectIntInput(firstYear) == -1)
                    {
                        errorMessage = "В полетата НАЧАЛО и КРАЙ трябва да се въведе година,а не дума!";
                        return;
                    }
                    else
                    {
                        firstYear = OftenUsedMethods.CorrectIntInput(firstYear).ToString();
                    }
                }
                else if (lastYear.Length > 0)
                {
                    if (OftenUsedMethods.CorrectIntInput(lastYear) == -1)
                    {
                        errorMessage = "В полетата НАЧАЛО и КРАЙ трябва да се въведе година,а не дума!";
                        return;
                    }
                    else
                    {
                        lastYear = OftenUsedMethods.CorrectIntInput(lastYear).ToString();
                    }
                }

                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query1 = "";

                        if (bookInfo.Title.Length > 2 && bookInfo.Author.Length == 0)
                        {
                            if (firstYear.Length > 0 && lastYear.Length > 0)
                            {
                                query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Year>=@begin and Year<=@end and Deduction='0';";
                            }
                            else if (firstYear.Length == 0 && lastYear.Length == 0)
                            {
                                query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Deduction='0';";
                            }
                            else if (firstYear.Length > 0 && lastYear.Length == 0)
                            {
                                query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Year>=@begin and Deduction='0';";
                            }
                            else if (firstYear.Length == 0 && lastYear.Length > 0)
                            {
                                query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Year<=@end and Deduction='0';";
                            }
                        }
                        else
                        {
                            if (bookInfo.Title.Length == 2 && bookInfo.Author.Length == 0)
                            {
                                if (firstYear.Length > 0 && lastYear.Length > 0)
                                {
                                    query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Year>=@begin and Year<=@end and Deduction='0';";
                                }
                                else if (firstYear.Length == 0 && lastYear.Length == 0)
                                {
                                    query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Deduction='0';";
                                }
                                else if (firstYear.Length > 0 && lastYear.Length == 0)
                                {
                                    query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Year>=@begin and Deduction='0';";
                                }
                                else if (firstYear.Length == 0 && lastYear.Length > 0)
                                {
                                    query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Year<=@end and Deduction='0';";
                                }
                            }
                            else
                            {
                                if (bookInfo.Title.Length > 2 && bookInfo.Author.Length != 0)
                                {
                                    if (firstYear.Length > 0 && lastYear.Length > 0)
                                    {
                                        query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Author=@author and Year>=@begin and Year<=@end and Deduction='0';";
                                    }
                                    else if (firstYear.Length == 0 && lastYear.Length == 0)
                                    {
                                        query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Author=@author and Deduction='0';";
                                    }
                                    else if (firstYear.Length > 0 && lastYear.Length == 0)
                                    {
                                        query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Author=@author and Year>=@begin and Deduction='0';";
                                    }
                                    else if (firstYear.Length == 0 && lastYear.Length > 0)
                                    {
                                        query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Title=@title and Author=@author and Year<=@end and Deduction='0';";
                                    }
                                }
                                else
                                {
                                    if (bookInfo.Title.Length == 2 && bookInfo.Author.Length != 0)
                                    {
                                        if (firstYear.Length > 0 && lastYear.Length > 0)
                                        {
                                            query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Author=@author and Year>=@begin and Year<=@end and Deduction='0';";
                                        }
                                        else if (firstYear.Length == 0 && lastYear.Length == 0)
                                        {
                                            query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Author=@author and Deduction='0';";
                                        }
                                        else if (firstYear.Length > 0 && lastYear.Length == 0)
                                        {
                                            query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Author=@author and Year>=@begin and Deduction='0';";
                                        }
                                        else if (firstYear.Length == 0 && lastYear.Length > 0)
                                        {
                                            query1 = "SELECT Title,Author,Year,IDLibrary,IsAvaiable,Category from [dbo].[Book] where Author=@author and Year<=@end and Deduction='0';";
                                        }
                                    }
                                }
                            }
                        }

                        using (SqlCommand command1 = new SqlCommand(query1, connection))
                        {
                            command1.Parameters.AddWithValue("@title", bookInfo.Title);
                            command1.Parameters.AddWithValue("@author", bookInfo.Author);
                            command1.Parameters.AddWithValue("@begin", firstYear.Trim());
                            command1.Parameters.AddWithValue("@end", lastYear.Trim());

                            using (SqlDataReader reader = command1.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    BookInformation booksInfo = new BookInformation();
                                    int year, idLib;

                                    booksInfo.Title = reader.GetString(0);
                                    booksInfo.Author = reader.GetString(1);
                                    year = reader.GetInt32(2);
                                    booksInfo.Year = year.ToString();
                                    idLib = reader.GetInt32(3);
                                    booksInfo.IdLibrary = idLib.ToString();
                                    booksInfo.IsAvaiable = reader.GetString(4);
                                    booksInfo.Category = reader.GetString(5);

                                    if (bookInfo.Category.Length != 0)
                                    {
                                        if (booksInfo.Category == bookInfo.Category)
                                        {
                                            if (bookInfo.IsAvaiable == "ДА")
                                            {
                                                if (booksInfo.IsAvaiable == "ДА")
                                                {
                                                    listBooks.Add(booksInfo);
                                                }
                                            }
                                            else
                                            {
                                                listBooks.Add(booksInfo);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (bookInfo.IsAvaiable == "ДА")
                                        {
                                            if (booksInfo.IsAvaiable == "ДА")
                                            {
                                                listBooks.Add(booksInfo);
                                            }
                                        }
                                        else
                                        {
                                            listBooks.Add(booksInfo);
                                        }
                                    }
                                }
                            }
                        }

                        string query2 = "";
                        List<int> unnecessaryId = new List<int>();
                        for (int i = 0; i < listBooks.Count; i++)
                        {
                            query2 = $"SELECT UserName,CityVillage,Municipality,District,Email,Telephone,Address from [dbo].[Library] WHERE ID={listBooks[i].IdLibrary}";

                            using (SqlCommand command2 = new SqlCommand(query2, connection))
                            {
                                using (SqlDataReader reader = command2.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        LibraryInformation libraryInfo = new LibraryInformation();

                                        libraryInfo.Name = reader.GetString(0);
                                        libraryInfo.CityVillage = reader.GetString(1);
                                        libraryInfo.Municipality = reader.GetString(2);
                                        libraryInfo.District = reader.GetString(3);
                                        libraryInfo.Email = reader.GetString(4);
                                        libraryInfo.Telephone = reader.GetString(5);
                                        libraryInfo.Address = reader.GetString(6);

                                        string url = ("https://www.google.com/maps/place/" + Uri.EscapeDataString(libraryInfo.Address));

                                        if (librariesInfo.District == "Без филтриране")
                                        {
                                            if (librariesInfo.CityVillage.Length == 0)
                                            {
                                                if (librariesInfo.Municipality.Length == 0)
                                                {
                                                    listLibraries.Add(libraryInfo);
                                                    addresses.Add(url);
                                                }
                                                else
                                                {
                                                    if (librariesInfo.Municipality == libraryInfo.Municipality)
                                                    {
                                                        listLibraries.Add(libraryInfo);
                                                        addresses.Add(url);
                                                    }
                                                    else
                                                    {
                                                        unnecessaryId.Add(i);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (librariesInfo.CityVillage == libraryInfo.CityVillage)
                                                {
                                                    if (librariesInfo.Municipality.Length == 0)
                                                    {
                                                        listLibraries.Add(libraryInfo);
                                                        addresses.Add(url);
                                                    }
                                                    else
                                                    {
                                                        if (librariesInfo.Municipality == libraryInfo.Municipality)
                                                        {
                                                            listLibraries.Add(libraryInfo);
                                                            addresses.Add(url);
                                                        }
                                                        else
                                                        {
                                                            unnecessaryId.Add(i);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    unnecessaryId.Add(i);
                                                }
                                            }
                                        }
                                        else if (librariesInfo.District == libraryInfo.District)
                                        {
                                            if (librariesInfo.CityVillage.Length == 0)
                                            {
                                                if (librariesInfo.Municipality.Length == 0)
                                                {
                                                    listLibraries.Add(libraryInfo);
                                                    addresses.Add(url);
                                                }
                                                else
                                                {
                                                    if (librariesInfo.Municipality == libraryInfo.Municipality)
                                                    {
                                                        listLibraries.Add(libraryInfo);
                                                        addresses.Add(url);
                                                    }
                                                    else
                                                    {
                                                        unnecessaryId.Add(i);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (librariesInfo.CityVillage == libraryInfo.CityVillage)
                                                {
                                                    if (librariesInfo.Municipality.Length == 0)
                                                    {
                                                        listLibraries.Add(libraryInfo);
                                                        addresses.Add(url);
                                                    }
                                                    else
                                                    {
                                                        if (librariesInfo.Municipality == libraryInfo.Municipality)
                                                        {
                                                            listLibraries.Add(libraryInfo);
                                                            addresses.Add(url);
                                                        }
                                                        else
                                                        {
                                                            unnecessaryId.Add(i);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    unnecessaryId.Add(i);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            unnecessaryId.Add(i);
                                        }

                                    }
                                }
                            }
                        }

                        int count = 0;
                        for (int i = 0; i < unnecessaryId.Count; i++)
                        {
                            listBooks.RemoveAt(unnecessaryId[i] - count);
                            count++;
                        }
                        connection.Close();
                    }
                    if (listBooks.Count == 0 || listLibraries.Count == 0)
                    {
                        errorMessage = "Не бяха открити резултати.";
                    }
                    else
                    {
                        successMessage = "Проверката е успешна.";
                    }
                }

                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
        }

        private void ClearLists()
        {
            listLibraries.Clear();
            listBooks.Clear();
            addresses.Clear();
        }
    }
}