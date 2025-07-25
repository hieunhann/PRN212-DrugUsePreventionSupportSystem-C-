using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows;

namespace druguseprevention
{
    public partial class ConsultantWindow : Window
    {
        public ConsultantWindow()
        {
            InitializeComponent();
            LoadConsultants();
        }

        private async void LoadConsultants()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Thêm token xác thực từ App.Current
                    var token = App.Current.Properties["token"]?.ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    var response = await client.GetAsync("http://localhost:8080/api/consultant/all-profiles");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var consultants = JsonConvert.DeserializeObject<List<ConsultantProfile>>(json);
                    ConsultantGrid.ItemsSource = consultants;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải tư vấn viên: " + ex.Message);
                }
            }
        }

    }
    public class ConsultantProfile
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
    }
}