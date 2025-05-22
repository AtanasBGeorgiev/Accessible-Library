using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.Pages
{
    public class ForgottenPassword2Model : PageModel
    {
        public static LibraryInformation libraryInfo = new LibraryInformation();
        private string _hashedPassword;

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            TimeSpan timeout = TimeSpan.FromMinutes(5);

            if (DateTime.Now - ForgottenPasswordModel.startTime > timeout)
            {
                errorMessage = "����� ���� �� � �������!��������� ��� ���.";
                return;
            }

            string code = Request.Form["code"];
            code = code.Trim();
            string password = Request.Form["password"];
            password = password.Trim();
            string confirmPassword = Request.Form["confirmPassword"];
            confirmPassword = confirmPassword.Trim();

            Regex validate = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[�-�])(?=.*?[�-�])(?=.*?[#?!@$%^&*-]).{8,}$");
            Regex validateAdmin = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[�-�])(?=.*?[�-�])(?=.*?[#?!@$%^&*-]).{15,}$");

            if (code.Length == 0 || password.Length == 0 || confirmPassword.Length == 0)
            {
                errorMessage = "������ ������ �� �����������!";
            }
            else
            {
                if (code.Trim() == "0") 
                {
                    errorMessage = "������� ���.";
                    return;
                }
                if (OftenUsedMethods.CorrectIntInput(code) == -1)  
                {
                    errorMessage = "����� � ������.";
                    return;
                }
                if (password != confirmPassword)
                {
                    errorMessage = "������������ ������ � �������� �� ��������!";
                    return;
                }
                if (ForgottenPasswordModel.verificationCode != int.Parse(code))
                {
                    errorMessage = "���������� ��� � ������!";
                    return;
                }
                if (ForgottenPasswordModel.database == "Administrators") 
                {
                    if (validateAdmin.IsMatch(password) == false)
                    {
                        errorMessage = "�������� �� �������� �� ������������!";
                        return;
                    }
                }
                else
                {
                    if (validate.IsMatch(password) == false)
                    {
                        errorMessage = "�������� �� �������� �� ������������!";
                        return;
                    }
                }               
                if (errorMessage.Length == 0)
                {
                    try
                    {
                        _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

                        string connectionString = OftenUsedMethods.ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            string sql = $"UPDATE [dbo].[{ForgottenPasswordModel.database}] SET Password = @password WHERE (ID = @id);";

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@password", _hashedPassword);
                                command.Parameters.AddWithValue("@id", ForgottenPasswordModel.id);

                                command.ExecuteNonQuery();
                            }
                            connection.Close();
                        }
                        successMessage = "�������� � ���������.";

                        ForgottenPasswordModel.verificationCode = 0;
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