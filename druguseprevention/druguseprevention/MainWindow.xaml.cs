using System.Windows;
using System.Windows.Controls;

namespace druguseprevention
{
    public partial class MainWindow : Window
    {
        // Giả lập trạng thái đăng nhập
        private bool IsLoggedIn = false;
        private string DisplayName = "Người dùng";

        public MainWindow()
        {
            InitializeComponent();
            UpdateAuthButtons();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string content = button.Content.ToString();

            if (!IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để sử dụng chức năng này!", "Thông báo");
                return;
            }

            switch (content)
            {
                case "Về chúng tôi":
                    MessageBox.Show("Đi tới trang Về chúng tôi");
                    break;
                case "Khóa học":
                    var courseWindow = new CourseListWindow();
                    courseWindow.ShowDialog();
                    break;
                case "Đánh giá":
                    var window = new CrafftWindow();
                    window.ShowDialog();
                    break;
                case "Tư vấn trực tuyến":
                    MessageBox.Show("Đi tới trang Tư vấn trực tuyến");
                    break;
                case "Chương trình cộng đồng":
                    MessageBox.Show("Đi tới trang Chương trình cộng đồng");
                    break;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đi tới trang Đăng ký");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            bool? result = loginWindow.ShowDialog();

            if (result == true)
            {
                IsLoggedIn = true;
                DisplayName = App.Current.Properties["userName"]?.ToString() ?? "Người dùng";
                UpdateAuthButtons();
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Xin chào {DisplayName}!\nBạn có thể xem hồ sơ cá nhân hoặc đăng xuất.");
            // Thêm logic dropdown tại đây nếu cần
        }

        private void UpdateAuthButtons()
        {
            var loginBtn = this.FindName("LoginButton") as Button;
            var registerBtn = this.FindName("RegisterButton") as Button;

            if (IsLoggedIn)
            {
                if (loginBtn != null) loginBtn.Visibility = Visibility.Collapsed;

                if (registerBtn != null)
                {
                    registerBtn.Content = DisplayName;
                    registerBtn.Click -= RegisterButton_Click;
                    registerBtn.Click += ProfileButton_Click;
                }
            }
            else
            {
                if (loginBtn != null) loginBtn.Visibility = Visibility.Visible;

                if (registerBtn != null)
                {
                    registerBtn.Content = "Đăng ký";
                    registerBtn.Click -= ProfileButton_Click;
                    registerBtn.Click += RegisterButton_Click;
                }
            }
        }
    }
}
