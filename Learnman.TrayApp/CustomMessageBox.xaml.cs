using System.Windows;

namespace Learnman.TrayApp
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message, string title)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Show(string message, string title = "LearnMan")
        {
            var msgBox = new CustomMessageBox(message, title);
            msgBox.ShowDialog();
        }
    }
}
