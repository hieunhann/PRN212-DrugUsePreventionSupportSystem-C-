using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace druguseprevention
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi");
                return;
            }

            var client = new HttpClient();
            var loginData = new
            {
                userName = username,
                password = password
            };

            string json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/api/login", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo");

                    App.Current.Properties["token"] = result.token;
                    App.Current.Properties["userName"] = result.userName;
                    App.Current.Properties["role"] = result.role;

                    if (result.role == "ADMIN")
                    {
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                        this.Close();
                        return;
                    }

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối tới máy chủ: " + ex.Message, "Lỗi kết nối");
            }
        }
    }

    public class LoginResponse
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public string phoneNumber { get; set; }
        public string address { get; set; }
        public string dateOfBirth { get; set; }
        public string gender { get; set; }
        public string role { get; set; }
        public string token { get; set; }
    }
}