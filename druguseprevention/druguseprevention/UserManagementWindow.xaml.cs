using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace druguseprevention
{
    public partial class UserManagementWindow : Window
    {
        private List<AdminProfileDTO> users = new List<AdminProfileDTO>();
        private AdminProfileDTO selectedUser = null;
        public UserManagementWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                var client = new HttpClient();
                string token = App.Current.Properties["token"]?.ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("http://localhost:8080/api/profile/all");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<AdminProfileDTO>>(json);
                    UserDataGrid.ItemsSource = users;
                }
                else
                {
                    MessageBox.Show("Không thể tải danh sách người dùng!", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        private void UserDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedUser = UserDataGrid.SelectedItem as AdminProfileDTO;
            if (selectedUser != null)
            {
                UserNameBox.Text = selectedUser.userName;
                FullNameBox.Text = selectedUser.fullName;
                EmailBox.Text = selectedUser.email;
                PhoneBox.Text = selectedUser.phoneNumber;
                AddressBox.Text = selectedUser.address;
                DobBox.Text = selectedUser.dateOfBirth;
                GenderBox.Text = selectedUser.gender;
                RoleBox.Text = selectedUser.role;
            }
        }

        private void AddNewUser_Click(object sender, RoutedEventArgs e)
        {
            selectedUser = null;
            UserNameBox.Text = "";
            FullNameBox.Text = "";
            EmailBox.Text = "";
            PhoneBox.Text = "";
            AddressBox.Text = "";
            DobBox.Text = "";
            GenderBox.Text = "";
            RoleBox.Text = "";
            UserDataGrid.UnselectAll();
        }

        private async void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            var user = new AdminProfileDTO
            {
                userId = selectedUser?.userId ?? 0,
                userName = UserNameBox.Text,
                fullName = FullNameBox.Text,
                email = EmailBox.Text,
                phoneNumber = PhoneBox.Text,
                address = AddressBox.Text,
                dateOfBirth = DobBox.Text,
                gender = GenderBox.Text,
                role = RoleBox.Text,
                deleted = false
            };
            var client = new HttpClient();
            string token = App.Current.Properties["token"]?.ToString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string json = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (selectedUser == null)
            {
                // Thêm mới
                response = await client.PostAsync("http://localhost:8080/api/profile/create-user", content);
            }
            else
            {
                // Sửa
                response = await client.PatchAsync("http://localhost:8080/api/profile/admin-update", content);
            }
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Lưu thành công!");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Lưu thất bại!");
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null)
            {
                MessageBox.Show("Chọn người dùng để xóa!");
                return;
            }
            if (MessageBox.Show($"Xác nhận xóa user {selectedUser.userName}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    var client = new HttpClient();
                    string token = App.Current.Properties["token"]?.ToString();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await client.DeleteAsync($"http://localhost:8080/api/profile/{selectedUser.userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Đã xóa thành công!");
                        LoadUsers();
                        AddNewUser_Click(null, null); // reset form
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa người dùng!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối: " + ex.Message);
                }
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            AddNewUser_Click(null, null);
        }
    }

    public class AdminProfileDTO
    {
        public long userId { get; set; }
        public string? userName { get; set; }
        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string? address { get; set; }
        public string? dateOfBirth { get; set; }
        public string? gender { get; set; }
        public string? role { get; set; }
        public bool deleted { get; set; }
    }
} 