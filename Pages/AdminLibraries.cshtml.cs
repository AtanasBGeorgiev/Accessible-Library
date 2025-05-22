using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class AdminLibrariesModel : PageModel
    {
        public static List<LibraryInformation> listLibraries = new List<LibraryInformation>();

        public string errorMessage = "";
        public void OnGet()
        {
            listLibraries.Clear();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT ID,UserName,CityVillage,Municipality,District,Email,Telephone,Address,DateOfCreation from [dbo].[Library];";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LibraryInformation library = new LibraryInformation();
                                int id;
                                DateTime date;

                                id = reader.GetInt32(0);
                                library.Id = id.ToString();
                                library.Name = reader.GetString(1);
                                library.CityVillage = reader.GetString(2);
                                library.Municipality = reader.GetString(3);
                                library.District = reader.GetString(4);
                                library.Email = reader.GetString(5);
                                library.Telephone = reader.GetString(6);
                                library.Address = reader.GetString(7);
                                date = reader.GetDateTime(8);
                                library.DateOfCreation = date.ToString();

                                listLibraries.Add(library);
                            }
                        }
                    }
                    connection.Close();
                }
                if (listLibraries.Count == 0)
                {
                    errorMessage = "Няма библиотеки.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
