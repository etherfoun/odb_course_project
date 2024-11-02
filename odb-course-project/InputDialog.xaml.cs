using System.Windows;

namespace odb_course_project
{
    public partial class InputDialog : Window
    {
        public string Answer { get; set; }
        public string Prompt { get; set; }

        public InputDialog(string prompt, string defaultValue = "")
        {
            InitializeComponent();
            Prompt = prompt;
            Answer = defaultValue;
            DataContext = this;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
