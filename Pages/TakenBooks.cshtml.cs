using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.SqlClient;
using System.Net;

namespace Library.Pages
{
    public class TakenBooksModel : PageModel
    {
        public string errorMessage = "";
        public string successMessage = "";
        public string specialMessage = "";

        public static List<BookInformation> listBooks = new List<BookInformation>();
        public static List<LibraryInformation> listLibraries = new List<LibraryInformation>();
        public static List<string> englishAddresses = new List<string>();

        public void OnGet()
        {
            listBooks.Clear();
            listLibraries.Clear();
            englishAddresses.Clear();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query1 = $"SELECT Title,Author,Year,IDLibrary,DateOfTaking from [dbo].[Book] WHERE IDReader={LoginReaderModel.readerInfo.Id};";
                    using (SqlCommand command1 = new SqlCommand(query1, connection))
                    {
                        using (SqlDataReader reader = command1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BookInformation bookInfo = new BookInformation();
                                int idLibrary, year;

                                bookInfo.Title = reader.GetString(0);
                                bookInfo.Author = reader.GetString(1);
                                year = reader.GetInt32(2);
                                bookInfo.Year = year.ToString();
                                idLibrary = reader.GetInt32(3);
                                bookInfo.IdLibrary = idLibrary.ToString();
                                bookInfo.DateOfTaking = reader.GetString(4);

                                listBooks.Add(bookInfo);
                            }
                        }
                    }

                    string query2;

                    for (int i = 0; i < listBooks.Count; i++)
                    {
                        query2 = $"SELECT UserName,CityVillage,Municipality,District,Email,Telephone,Address from [dbo].[Library] where ID=@id";
                        using (SqlCommand command2 = new SqlCommand(query2, connection))
                        {
                            command2.Parameters.AddWithValue("@id", listBooks[i].IdLibrary);
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

                                    listLibraries.Add(libraryInfo);

                                    string url = ("https://www.google.com/maps/place/" + Uri.EscapeDataString(libraryInfo.Address));
                                    englishAddresses.Add(url);
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                TimeSpan ts;
                for (int i = 0; i < listBooks.Count; i++) 
                {
                    ts = DateTime.Now - Convert.ToDateTime(listBooks[i].DateOfTaking);
                    if (ts.TotalDays > 30) 
                    {
                        specialMessage = "Просрочена";
                        break;
                    }
                }
                if (listBooks.Count == 0) 
                {
                    successMessage = "Не си взел книга/и.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}