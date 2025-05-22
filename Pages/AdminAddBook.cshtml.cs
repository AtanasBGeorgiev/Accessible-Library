using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class AdminAddBookModel : PageModel
    {
        public static List<BookInformation> listBooks = new List<BookInformation>();

        public static BookInformation bookInfo = new BookInformation();

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            bookInfo.InventoryNum = Request.Form["inventoryNum"];
            bookInfo.Title = Request.Form["title"];
            bookInfo.Author = Request.Form["author"];
            bookInfo.Year = Request.Form["year"];
            bookInfo.Price = Request.Form["price"];
            bookInfo.Signature = Request.Form["signature"];
            bookInfo.Category = Request.Form["category"];

            if (bookInfo.InventoryNum.Length == 0 || bookInfo.Title.Length == 0 || bookInfo.Author.Length == 0 ||
                bookInfo.Year.Length == 0 || bookInfo.Price.Length == 0 || bookInfo.Signature.Length == 0)
            {
                errorMessage = "������ ������ �� ������������!";
                return;
            }

            else if (OftenUsedMethods.CorrectTopic(bookInfo) == "������")
            {
                errorMessage = "���������� �� ������� ������ �� � � �������!";
                return;
            }
            else if (OftenUsedMethods.CorrectNameAndFamilia(bookInfo.Author) == "���� ���.")
            {
                errorMessage = "�� ���� ������� �� ��� ���� ���� ���!";
                return;
            }
            else if (OftenUsedMethods.CorrectIntInput(bookInfo.Year) == -1 ||
                OftenUsedMethods.CorrectDoubleInput(bookInfo.Price) == -1)
            {
                errorMessage = "� �������� ������ � ���� ������ �� �� ������ �����!";
                return;
            }
            else
            {
                bookInfo.Title = OftenUsedMethods.CorrectTopic(bookInfo);
                bookInfo.Author = OftenUsedMethods.CorrectNameAndFamilia(bookInfo.Author);
                int year = OftenUsedMethods.CorrectIntInput(bookInfo.Year);

                /*����������� �� ����� � double, �� �� ���� ������� �� ���������� ��� 
                  ���������������.
                  ��. ������� �� 14,40
                  (float)=> 14,399999618530273
                  (double)=> 14,4
                  ��. ������� �� 15,30
                  (float)=> 15,300000190734863
                  (double)=> 15,3 */
                double price = OftenUsedMethods.CorrectDoubleInput(bookInfo.Price);

                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        connection.Open();

                        string sql = $"Select count(*) from [dbo].[Book] where InventoryNum=@inventoryNum and IDLibrary={AdminLibraryInfoModel.libraryInfo.Id}";
                        bool exist = false;

                        using (SqlCommand command1 = new SqlCommand(sql, connection))
                        {
                            command1.Parameters.AddWithValue("@inventoryNum", bookInfo.InventoryNum);

                            command1.ExecuteNonQuery();
                            exist = (int)command1.ExecuteScalar() > 0;
                        }

                        if (exist == false)
                        {
                            string query = "INSERT INTO [dbo].[Book] (InventoryNum,Title,Author,Year," +
                                "Price,Signature,IDLibrary,IsAvaiable,DateOfTaking,Inventory,Deduction,Category,SentEmail) VALUES (@inventoryNum," +
                                "@title,@author,@year,@price,@signature,@idLibrary,@isAvaiable,'0','0','0',@category,'0');";
                            using (SqlCommand command2 = new SqlCommand(query, connection))
                            {
                                command2.Parameters.AddWithValue("@inventoryNum", bookInfo.InventoryNum.Trim());
                                command2.Parameters.AddWithValue("@title", bookInfo.Title);
                                command2.Parameters.AddWithValue("@author", bookInfo.Author);
                                command2.Parameters.AddWithValue("@year", year);
                                command2.Parameters.AddWithValue("@price", price);
                                command2.Parameters.AddWithValue("@signature", bookInfo.Signature.Trim());
                                command2.Parameters.AddWithValue("@idLibrary", AdminLibraryInfoModel.libraryInfo.Id);
                                command2.Parameters.AddWithValue("@isAvaiable", "��");
                                command2.Parameters.AddWithValue("@category", bookInfo.Category);

                                command2.ExecuteNonQuery();

                                successMessage = "���� ����� � �������� �������.";
                            }
                        }
                        else
                        {
                            errorMessage = "���� ��������e� ����� ���� ����������!";
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
