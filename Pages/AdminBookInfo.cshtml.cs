using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class AdminBookInfoModel : PageModel
    {
        public static BookInformation bookInfo = new BookInformation();

        private static int _id;
        private static string _inventoryNum;
        private static string _title;
        private static string _author;
        private static string _year;
        private static string _price;
        private static string _signature;
        private static string _inventory;
        private static string _category;

        private double _price2;
        private int _year2;

        public string successMessage = "";
        public string errorMessage = "";

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
                                DateTime date;

                                bookInfo.Id = id;
                                _inventoryNum = reader.GetString(1);
                                bookInfo.InventoryNum = _inventoryNum;
                                _title = reader.GetString(2);
                                bookInfo.Title = _title;
                                _author = reader.GetString(3);
                                bookInfo.Author = _author;
                                _year = reader.GetInt32(4).ToString();
                                bookInfo.Year = _year;
                                _price = reader.GetDouble(5).ToString();
                                bookInfo.Price = _price;
                                _signature = reader.GetString(6);
                                bookInfo.Signature = _signature;
                                _inventory = reader.GetString(11);
                                bookInfo.Inventory = _inventory;
                                date = reader.GetDateTime(13);
                                bookInfo.DateOfCreation = date.ToString();
                                _category = reader.GetString(15);
                                bookInfo.Category = _category;
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
            bookInfo.InventoryNum = Request.Form["inventoryNum"];
            bookInfo.Title = Request.Form["title"];
            bookInfo.Author = Request.Form["author"];
            bookInfo.Year = Request.Form["year"];
            bookInfo.Price = Request.Form["price"];
            bookInfo.Signature = Request.Form["signature"];
            bookInfo.Inventory = Request.Form["inventory"];
            bookInfo.Category = Request.Form["category"];

            if (bookInfo.InventoryNum.Trim().Length == 0 || bookInfo.Title.Length == 0 || bookInfo.Author.Length == 0 ||
                bookInfo.Year.Length == 0 || bookInfo.Price.Length == 0 || bookInfo.Signature.Trim().Length == 0 ||
                bookInfo.Inventory.Trim().Length == 0 || bookInfo.Category.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                if (bookInfo.InventoryNum == _inventoryNum && bookInfo.Title == _title && bookInfo.Author == _author &&
                    bookInfo.Year == _year && bookInfo.Price == _price && bookInfo.Signature == _signature &&
                    bookInfo.Inventory == _inventory && bookInfo.Category == _category)
                {
                    successMessage = "Не беше променена информация.";
                }
                else
                {
                    if (OftenUsedMethods.CorrectTopic(bookInfo) == "Грешка")
                    {
                        errorMessage = "Заглавието на книгата трябва да е в кавички!";
                        return;
                    }
                    else if (OftenUsedMethods.CorrectNameAndFamilia(bookInfo.Author) == "Едно име.")
                    {
                        errorMessage = "Не може авторът да има само едно име!";
                        return;
                    }
                    else if (OftenUsedMethods.CorrectIntInput(bookInfo.Year) == -1 ||
                        OftenUsedMethods.CorrectDoubleInput(bookInfo.Price) == -1)
                    {
                        errorMessage = "В полетата ГОДИНА и ЦЕНА трябва да е въведено число!";
                        return;
                    }

                    if (errorMessage.Length > 0)
                    {
                        bookInfo.InventoryNum = _inventoryNum;
                        bookInfo.Title = _title;
                        bookInfo.Author = _author;
                        bookInfo.Year = _year;
                        bookInfo.Price = _price;
                        bookInfo.Signature = _signature;
                        bookInfo.Inventory = _inventory;
                        bookInfo.Category = _category;
                    }
                    else
                    {
                        bookInfo.Title = OftenUsedMethods.CorrectTopic(bookInfo);
                        bookInfo.Author = OftenUsedMethods.CorrectNameAndFamilia(bookInfo.Author);
                        _year2 = OftenUsedMethods.CorrectIntInput(bookInfo.Year);
                        _price2 = OftenUsedMethods.CorrectDoubleInput(bookInfo.Price);

                        try
                        {
                            string connectionString = OftenUsedMethods.ConnectionString;
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                string sql = "Select count(*) from [dbo].[Book] where InventoryNum=@inventoryNum and IDLibrary=@idLibrary";
                                int count = 0;

                                using (SqlCommand command1 = new SqlCommand(sql, connection))
                                {
                                    command1.Parameters.AddWithValue("@inventoryNum", bookInfo.InventoryNum.Trim());
                                    command1.Parameters.AddWithValue("@idLibrary", AdminLibraryInfoModel.libraryInfo.Id);

                                    command1.ExecuteNonQuery();
                                    count = (int)command1.ExecuteScalar();
                                }

                                if (bookInfo.InventoryNum == _inventoryNum)
                                {
                                    string query = $" UPDATE [dbo].[Book] SET Title=@title,Author=@author,Year=@year,Price=@price,Signature=@signature,Inventory=@inventory,Category=@category WHERE (ID = @id)";
                                    using (SqlCommand command2 = new SqlCommand(query, connection))
                                    {
                                        command2.Parameters.AddWithValue("@title", bookInfo.Title);
                                        command2.Parameters.AddWithValue("@author", bookInfo.Author);
                                        command2.Parameters.AddWithValue("@year", _year2);
                                        command2.Parameters.AddWithValue("@price", _price2);
                                        command2.Parameters.AddWithValue("@signature", bookInfo.Signature.Trim());
                                        command2.Parameters.AddWithValue("@inventory", bookInfo.Inventory.Trim());
                                        command2.Parameters.AddWithValue("@category", bookInfo.Category);
                                        command2.Parameters.AddWithValue("@id", _id);

                                        command2.ExecuteNonQuery();

                                        successMessage = "Промени информацията за тази книга.";
                                    }
                                }
                                else
                                {
                                    if (count < 1)
                                    {
                                        string query = $" UPDATE [dbo].[Book] SET InventoryNum=@inventoryNum,Title=@title,Author=@author,Year=@year,Category=@category WHERE (ID = @id)";
                                        using (SqlCommand command2 = new SqlCommand(query, connection))
                                        {
                                            command2.Parameters.AddWithValue("@inventoryNum", bookInfo.InventoryNum.Trim());
                                            command2.Parameters.AddWithValue("@title", bookInfo.Title);
                                            command2.Parameters.AddWithValue("@author", bookInfo.Author);
                                            command2.Parameters.AddWithValue("@year", _year2);
                                            command2.Parameters.AddWithValue("@price", _price2);
                                            command2.Parameters.AddWithValue("@signature", bookInfo.Signature.Trim());
                                            command2.Parameters.AddWithValue("@inventory", bookInfo.Inventory.Trim());
                                            command2.Parameters.AddWithValue("@category", bookInfo.Category);
                                            command2.Parameters.AddWithValue("@id", _id);

                                            command2.ExecuteNonQuery();

                                            successMessage = "Промени информацията за тази книга.";
                                        }
                                    }
                                    else
                                    {
                                        errorMessage = "Този инвентарeн номер вече съществува!";
                                        bookInfo.InventoryNum = _inventoryNum;
                                        bookInfo.Title = _title;
                                        bookInfo.Author = _author;
                                        bookInfo.Year = _year;
                                        bookInfo.Price = _price;
                                        bookInfo.Signature = _signature;
                                        bookInfo.Inventory = _inventory;
                                        bookInfo.Category = _category;
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
                }
            }
        }
    }
}
