using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace druguseprevention
{
    public partial class LessonWindow : Window
    {
        private string materialUrl;

        public LessonWindow(int lessonId)
        {
            InitializeComponent();
            LoadLesson(lessonId);
        }

        private async void LoadLesson(int lessonId)
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
                var response = await client.GetAsync($"http://localhost:8080/api/lessons/{lessonId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var lesson = JsonSerializer.Deserialize<Lesson>(json);

                TitleBlock.Text = $"{lesson.title}";
                ContentBlock.Text = $"{lesson.content}";
                materialUrl = lesson.materialUrl;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bài học: " + ex.Message);
            }
        }
       

        private void OpenMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(materialUrl))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = materialUrl,
                    UseShellExecute = true
                });
            }
        }
        private async void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new HttpClient();
                var token = App.Current.Properties["token"]?.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.GetAsync("http://localhost:8080/api/quiz/course/1");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var quizList = JsonSerializer.Deserialize<List<Quiz>>(json);

                var quizWindow = new CourseQuizWindow(quizList);
                quizWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bài kiểm tra: " + ex.Message);
            }
        }

    }

    public class Lesson
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string materialUrl { get; set; }
        public int lessonOrder { get; set; }
        public Courses course { get; set; }
    }

    public class Courses
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
    public class Quiz
    {
        public int id { get; set; }
        public int courseId { get; set; }
        public string question { get; set; }
        public string answer { get; set; } 
        public int correct { get; set; }
        public List<string> AnswerOptions => JsonSerializer.Deserialize<List<string>>(answer);

    }
}