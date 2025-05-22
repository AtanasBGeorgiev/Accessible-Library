using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace Library.Pages
{
    public class BrowseLibraryModel : PageModel
    {
        public static List<string> libraryNames = new List<string>();
        public static List<string> cities = new List<string>();
        public static List<string> municipalities = new List<string>();
        public static List<string> districts = new List<string>();

        LibraryInformation libraryInfo = new LibraryInformation();
        public static List<BookInformation> books = new List<BookInformation>();

        private int _idLibrary;
        public static string choosenLibrary = "";
        public static string choosenDistrict = "";
        public static string choosenMunicipality = "";
        public static string choosenCity = "";
        private static string _clear;
        private static string _isAvaiable;
        public static string category;

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            ClearLists();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT UserName,CityVillage,Municipality,District from [dbo].[Library]";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LibraryInformation library = new LibraryInformation();

                                library.Name = reader.GetString(0);
                                library.CityVillage = reader.GetString(1);
                                library.Municipality = reader.GetString(2);
                                library.District = reader.GetString(3);

                                libraryNames.Add(library.Name);

                                int countCities = 0, countMunicipalities = 0, countDistricts = 0;
                                for (int i = 0; i < cities.Count; i++)
                                {
                                    if (library.CityVillage == cities[i])
                                    {
                                        countCities++;
                                    }
                                }
                                if (countCities == 0)
                                {
                                    cities.Add(library.CityVillage);
                                }

                                for (int i = 0; i < municipalities.Count; i++)
                                {
                                    if (library.Municipality == municipalities[i])
                                    {
                                        countMunicipalities++;
                                    }
                                }
                                if (countMunicipalities == 0)
                                {
                                    municipalities.Add(library.Municipality);
                                }

                                for (int i = 0; i < districts.Count; i++)
                                {
                                    if (library.District == districts[i])
                                    {
                                        countDistricts++;
                                    }
                                }
                                if (countDistricts == 0)
                                {
                                    districts.Add(library.District);
                                }
                            }
                        }
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

        public void OnPost()
        {
            _clear = Request.Form["clear"];

            if (_clear == "ДА")
            {
                choosenLibrary = "";
                choosenDistrict = "";
                choosenMunicipality = "";
                choosenCity = "";
                category = "";

                OnGet();
            }
            else
            {
                _isAvaiable = Request.Form["isAvaiable"];
                libraryInfo.Name = Request.Form["library"];
                choosenLibrary = libraryInfo.Name;
                libraryInfo.District = Request.Form["district"];
                choosenDistrict = libraryInfo.District;
                libraryInfo.Municipality = Request.Form["municipality"];
                choosenMunicipality = libraryInfo.Municipality;
                libraryInfo.CityVillage = Request.Form["cityVillage"];
                choosenCity = libraryInfo.CityVillage;
                category = Request.Form["category"];

                if (libraryInfo.Name.Length > 0)
                {
                    books.Clear();

                    try
                    {
                        string connectionString = OftenUsedMethods.ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            string query1 = "SELECT ID FROM [dbo].[Library] WHERE UserName=@library";          
                            using (SqlCommand command1 = new SqlCommand(query1, connection))
                            {
                                command1.Parameters.AddWithValue("@library", libraryInfo.Name);
                                _idLibrary = (int)command1.ExecuteScalar();
                            }

                            string query2 = "";
                            string query3 = "";
                            if (category.Length > 0)
                            {
                                query2 = $"SELECT Title,Author,Year,IsAvaiable FROM [dbo].[Book] WHERE IDLibrary={_idLibrary} and Category='{category}';";
                                query3 = $"SELECT count(*) FROM [dbo].[Book] WHERE IDLibrary={_idLibrary} and Category='{category}';";
                            }
                            else
                            {
                                query2 = $"SELECT Title,Author,Year,IsAvaiable FROM [dbo].[Book] WHERE IDLibrary={_idLibrary}";
                            }

                            if (query3.Length == 0)
                            {
                                using (SqlCommand command2 = new SqlCommand(query2, connection))
                                {
                                    using (SqlDataReader reader = command2.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            BookInformation bookInfo = new BookInformation();

                                            int year;
                                            bookInfo.Title = reader.GetString(0);
                                            bookInfo.Author = reader.GetString(1);
                                            year = reader.GetInt32(2);
                                            bookInfo.Year = year.ToString();
                                            bookInfo.IsAvaiable = reader.GetString(3);

                                            //Добавят се само налични книги
                                            if (_isAvaiable == "ДА")
                                            {
                                                if (bookInfo.IsAvaiable == "ДА")
                                                    books.Add(bookInfo);
                                            }
                                            else
                                                //Добавят се всички книги
                                                books.Add(bookInfo);

                                        }
                                        successMessage = "Проверката е успешна.";
                                    }
                                }
                            }
                            else
                            {
                                bool exist;
                                using (SqlCommand command3 = new SqlCommand(query3, connection))
                                {
                                    exist = (int)command3.ExecuteScalar() > 0;
                                }

                                if (exist == false) 
                                {
                                    errorMessage = "Не са открити резултати.";
                                }
                                else
                                {
                                    using (SqlCommand command2 = new SqlCommand(query2, connection))
                                    {
                                        using (SqlDataReader reader = command2.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                BookInformation bookInfo = new BookInformation();

                                                int year;
                                                bookInfo.Title = reader.GetString(0);
                                                bookInfo.Author = reader.GetString(1);
                                                year = reader.GetInt32(2);
                                                bookInfo.Year = year.ToString();
                                                bookInfo.IsAvaiable = reader.GetString(3);

                                                //Добавят се само налични книги
                                                if (_isAvaiable == "ДА")
                                                {
                                                    if (bookInfo.IsAvaiable == "ДА")
                                                        books.Add(bookInfo);
                                                }
                                                else
                                                    //Добавят се всички книги
                                                    books.Add(bookInfo);

                                            }
                                            successMessage = "Проверката е успешна.";
                                        }
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
                else
                {
                    string query1 = "";
                    string query2 = "";
                    int option = 0;

                    if (libraryInfo.District.Length > 0 && libraryInfo.Municipality.Length > 0 && libraryInfo.CityVillage.Length > 0)
                    {
                        query1 = "SELECT UserName from [dbo].[Library] where District=@district and Municipality=@municipality and CityVillage=@cityVillage;";
                    }
                    else if (libraryInfo.District.Length > 0 && libraryInfo.Municipality.Length == 0 && libraryInfo.CityVillage.Length == 0)
                    {
                        query1 = "SELECT UserName from [dbo].[Library] where District=@district;";
                        query2 = $"SELECT Municipality,CityVillage from [dbo].[Library] where District=@district;";
                        option = 1;
                    }
                    else if (libraryInfo.District.Length == 0 && libraryInfo.Municipality.Length > 0 && libraryInfo.CityVillage.Length == 0)
                    {
                        query1 = "SELECT UserName from [dbo].[Library] where Municipality=@municipality;";
                        query2 = $"SELECT CityVillage from [dbo].[Library] where Municipality=@municipality;";
                        option = 2;
                    }
                    else if (libraryInfo.District.Length == 0 && libraryInfo.Municipality.Length == 0 && libraryInfo.CityVillage.Length > 0)
                    {
                        query1 = "SELECT UserName from [dbo].[Library] where CityVillage=@cityVillage;";
                    }
                    else if (libraryInfo.District.Length > 0 && libraryInfo.Municipality.Length > 0 && libraryInfo.CityVillage.Length == 0)
                    {
                        query1 = "SELECT UserName from [dbo].[Library] where District=@district and Municipality=@municipality;";
                        query2 = $"SELECT CityVillage from [dbo].[Library] where Municipality=@municipality;";
                        option = 2;
                    }
                    else if (libraryInfo.District.Length > 0 && libraryInfo.Municipality.Length == 0 && libraryInfo.CityVillage.Length > 0)
                    {
                        query1 = "SELECT UserName from [dbo].[Library] where District=@district and CityVillage=@cityVillage;";
                    }
                    else if (libraryInfo.District.Length == 0 && libraryInfo.Municipality.Length > 0 && libraryInfo.CityVillage.Length > 0)
                    {
                        query1 = "SELECT UserName from [dbo].[Library] where Municipality=@municipality and CityVillage=@cityVillage;";
                    }
                    else
                    {
                        errorMessage = "Няма избрани критерии,по които да се търси библиотека.Всички книги са в списъка.";
                        return;
                    }

                    libraryNames.Clear();
                    books.Clear();

                    try
                    {
                        string connectionString = OftenUsedMethods.ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            using (SqlCommand command1 = new SqlCommand(query1, connection))
                            {
                                command1.Parameters.AddWithValue("@district", libraryInfo.District);
                                command1.Parameters.AddWithValue("@municipality", libraryInfo.Municipality);
                                command1.Parameters.AddWithValue("@cityVillage", libraryInfo.CityVillage);

                                using (SqlDataReader reader = command1.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string userName;
                                        userName = reader.GetString(0);

                                        libraryNames.Add(userName);
                                    }
                                }
                            }
                            if (query2.Length > 0)
                            {
                                using (SqlCommand command2 = new SqlCommand(query2, connection))
                                {
                                    command2.Parameters.AddWithValue("@district", libraryInfo.District);
                                    command2.Parameters.AddWithValue("@municipality", libraryInfo.Municipality);

                                    using (SqlDataReader reader = command2.ExecuteReader())
                                    {
                                        if (option == 1)
                                        {
                                            municipalities.Clear();
                                            cities.Clear();

                                            while (reader.Read())
                                            {
                                                LibraryInformation library = new LibraryInformation();

                                                library.Municipality = reader.GetString(0);
                                                library.CityVillage = reader.GetString(1);

                                                int countCities = 0, countMunicipalities = 0;
                                                for (int i = 0; i < cities.Count; i++)
                                                {
                                                    if (library.CityVillage == cities[i])
                                                    {
                                                        countCities++;
                                                    }
                                                }
                                                if (countCities == 0)
                                                {
                                                    cities.Add(library.CityVillage);
                                                }

                                                for (int i = 0; i < municipalities.Count; i++)
                                                {
                                                    if (library.Municipality == municipalities[i])
                                                    {
                                                        countMunicipalities++;
                                                    }
                                                }
                                                if (countMunicipalities == 0)
                                                {
                                                    municipalities.Add(library.Municipality);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            cities.Clear();

                                            while (reader.Read())
                                            {
                                                LibraryInformation library = new LibraryInformation();

                                                library.CityVillage = reader.GetString(0);

                                                int countCities = 0;
                                                for (int i = 0; i < cities.Count; i++)
                                                {
                                                    if (library.CityVillage == cities[i])
                                                    {
                                                        countCities++;
                                                    }
                                                }
                                                if (countCities == 0)
                                                {
                                                    cities.Add(library.CityVillage);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
                        if (libraryNames.Count == 0)
                        {
                            errorMessage = "Не са открити резултати.";
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }

                }
            }
        }
        private void ClearLists()
        {
            libraryNames.Clear();
            districts.Clear();
            municipalities.Clear();
            cities.Clear();
        }      
    }
}