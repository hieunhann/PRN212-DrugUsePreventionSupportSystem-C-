using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace druguseprevention
{
    public partial class CourseQuizWindow : Window
    {
        private List<Quiz> quizzes = new();

        public CourseQuizWindow(List<Quiz> quizData)
        {
            InitializeComponent();
            quizzes = quizData;
            LoadQuiz();
        }

        private void LoadQuiz()
        {
            for (int i = 0; i < quizzes.Count; i++)
            {
                var quiz = quizzes[i];

                var groupBox = new GroupBox
                {
                    Header = $"Câu {i + 1}: {quiz.question}",
                    Margin = new Thickness(0, 0, 0, 20)
                };

                var stack = new StackPanel();
                var options = JsonSerializer.Deserialize<List<string>>(quiz.answer);

                for (int j = 0; j < options.Count; j++)
                {
                    var radio = new RadioButton
                    {
                        Content = options[j],
                        GroupName = $"Quiz_{quiz.id}",
                        Tag = j
                    };
                    stack.Children.Add(radio);
                }

                groupBox.Content = stack;
                QuizPanel.Children.Add(groupBox);
            }

            var submitButton = new Button
            {
                Content = "Nộp bài",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0)
            };
            submitButton.Click += SubmitQuiz;
            QuizPanel.Children.Add(submitButton);
        }

        private void SubmitQuiz(object sender, RoutedEventArgs e)
        {
            int score = 0;

            foreach (var child in QuizPanel.Children)
            {
                if (child is GroupBox groupBox && groupBox.Content is StackPanel stack)
                {
                    foreach (var option in stack.Children)
                    {
                        if (option is RadioButton radio && radio.IsChecked == true)
                        {
                            var quizId = int.Parse(radio.GroupName.Split("_")[1]);
                            var quiz = quizzes.Find(q => q.id == quizId);
                            int selected = (int)radio.Tag;
                            if (selected == quiz.correct)
                                score++;
                        }
                    }
                }
            }

            MessageBox.Show($"Bạn trả lời đúng {score}/{quizzes.Count} câu!");
        }
    }
}