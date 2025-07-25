using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace druguseprevention
{
    public partial class CrafftWindow : Window
    {
        private List<Question> Questions = new();
        private Dictionary<int, int> Answers = new(); // questionId -> answerId

        public CrafftWindow()
        {
            InitializeComponent();
            LoadQuestionsAsync();
        }

        private async Task LoadQuestionsAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                App.Current.Properties["token"]?.ToString());

            try
            {
                var res = await client.PostAsync("http://localhost:8080/api/assessments/start?type=CRAFFT", null);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CrafftResponse>(json);
                    Questions = result.questions;
                    RenderQuestions();
                }
                else
                {
                    MessageBox.Show("Không có câu hỏi nào được tải.");
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Không thể kết nối tới máy chủ.");
                Close();
            }
        }

        private void RenderQuestions()
        {
            QuestionsPanel.Children.Clear();

            int index = 1;
            foreach (var question in Questions)
            {
                var group = new StackPanel
                {
                    Margin = new Thickness(0, 0, 0, 15)
                };

                var title = new TextBlock
                {
                    Text = $"{index}. {question.questionText}",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                group.Children.Add(title);

                foreach (var answer in question.answers)
                {
                    var radio = new RadioButton
                    {
                        GroupName = $"question_{question.id}",
                        Content = answer.text,
                        Tag = new TagData { QuestionId = question.id, AnswerId = answer.id },
                        Margin = new Thickness(10, 0, 0, 5)
                    };
                    radio.Checked += Radio_Checked;
                    group.Children.Add(radio);
                }

                QuestionsPanel.Children.Add(group);
                index++;
            }
        }

        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio && radio.Tag is object tagObj)
            {
                var tag = (TagData)tagObj;
                Answers[tag.QuestionId] = tag.AnswerId;
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Answers.Count < Questions.Count)
            {
                MessageBox.Show("Vui lòng trả lời tất cả câu hỏi!");
                return;
            }

            var payload = new List<object>();
            foreach (var entry in Answers)
            {
                payload.Add(new { questionId = entry.Key, answerId = entry.Value });
            }

            string json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                App.Current.Properties["token"]?.ToString());

            try
            {
                var res = await client.PostAsync("http://localhost:8080/api/assessments/submit?type=CRAFFT", content);
                if (res.IsSuccessStatusCode)
                {
                    var resultJson = await res.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SubmitResponse>(resultJson);

                    MessageBox.Show(
                        $"Đánh giá đã được gửi thành công!\n\n" +
                        $"ID: {result.assessmentResultId}\n" +
                        $"Điểm: {result.score}\n" +
                        $"Lời khuyên: {result.recommendation}",
                        "Kết quả đánh giá",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    Close();
                }
                else
                {
                    MessageBox.Show("Gửi đánh giá thất bại!");
                }
            }
            catch
            {
                MessageBox.Show("Không thể gửi bài đánh giá.");
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class CrafftResponse
    {
        public List<Question> questions { get; set; }
    }

    public class Question
    {
        public int id { get; set; }
        public string questionText { get; set; }
        public List<Answer> answers { get; set; }
    }

    public class Answer
    {
        public int id { get; set; }
        public string text { get; set; }
    }

    public class TagData
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
    public class SubmitResponse
    {
        public int assessmentResultId { get; set; }
        public int score { get; set; }
        public string recommendation { get; set; }
    }


}
