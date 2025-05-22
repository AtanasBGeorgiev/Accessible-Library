using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class DeductedBooksLibraryModel : PageModel
    {
        public static List<BookInformation> listBooks = new List<BookInformation>();
        public string errorMessage = "";
        public void OnGet()
        {
            listBooks.Clear();

            try
			{
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT InventoryNum,Title,Author,Year,Price,Signature,Inventory,Deduction,DateOfCreation FROM [dbo].[Book] where IDLibrary={LoginLibraryModel.libraryInfo.Id} and Deduction!='0';";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader()) 
                        {
                            while (reader.Read()) 
                            {
                                BookInformation book = new BookInformation();
                                int year;
                                double price;
                                DateTime date;

                                book.InventoryNum = reader.GetString(0);
                                book.Title = reader.GetString(1);
                                book.Author = reader.GetString(2);
                                year = reader.GetInt32(3);
                                book.Year = year.ToString();
                                price = reader.GetDouble(4);
                                book.Price = price.ToString();
                                book.Signature = reader.GetString(5);
                                book.Inventory = reader.GetString(6);
                                book.Deduction = reader.GetString(7);
                                date = reader.GetDateTime(8);
                                book.DateOfCreation = date.ToString();

                                listBooks.Add(book);
                            }
                        }
                    }
                    connection.Close();
                }
                if (listBooks.Count == 0)
                {
                    errorMessage = "Няма отчислени книги.";
                }
            }
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
        }
    }
}