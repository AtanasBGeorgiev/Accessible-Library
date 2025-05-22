using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class AdminReadersModel : PageModel
    {
        public static List<Reader> listReaders = new List<Reader>();

        public string errorMessage = "";

        public void OnGet()
        {
            listReaders.Clear();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT ID,Name,LastName,Email,Telephone,DateOfCreation from [dbo].[Reader];";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Reader person = new Reader();
                                int id;
                                DateTime date;

                                id = reader.GetInt32(0);
                                person.Id = id.ToString();
                                person.Name = reader.GetString(1);
                                person.LastName = reader.GetString(2);
                                person.Email = reader.GetString(3);
                                person.Telephone = reader.GetString(4);
                                date = reader.GetDateTime(5);
                                person.DateOfCreation = date.ToString();

                                listReaders.Add(person);
                            }
                        }
                    }
                    connection.Close();
                }
                if (listReaders.Count == 0)
                {
                    errorMessage = "Няма читатели.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
