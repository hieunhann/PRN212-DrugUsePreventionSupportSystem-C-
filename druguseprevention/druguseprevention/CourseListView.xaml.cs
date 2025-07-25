using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace druguseprevention
{
    public partial class CourseListWindow : Window
    {
        public CourseListWindow()
        {
            InitializeComponent();
            LoadCoursesAsync();
        }

        private async Task LoadCoursesAsync()
        {
            var client = new HttpClient();
            var token = App.Current.Properties["token"]?.ToString();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            try
            {
                var response = await client.GetAsync("http://localhost:8080/api/courses/list");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var courses = JsonSerializer.Deserialize<List<Course>>(json);
                CourseListView.ItemsSource = courses;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Không thể tải danh sách khóa học: " + ex.Message, "Lỗi");
            }
        }

        private void CourseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCourse = CourseListView.SelectedItem as Course;
            if (selectedCourse != null)
            {
                var confirm = MessageBox.Show($"Bạn muốn tham gia khóa học \"{selectedCourse.name}\"?", "Xác nhận", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    var lessonWindow = new LessonWindow(selectedCourse.id); // Giả sử mỗi khóa có ít nhất 1 bài học với ID bằng ID khóa học
                    lessonWindow.ShowDialog();
                }
            }
        }

        public class Course
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string targetAgeGroup { get; set; }
            public string url { get; set; }
            public bool deleted { get; set; }
        }
    }
}