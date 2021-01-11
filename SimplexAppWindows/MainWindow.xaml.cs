using System.Windows;
using System.Windows.Input;
namespace SimplexAppWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //При нажатии закрывает проект.//When the user has already clicked the button the app will closed
        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        //При нажатии сворачивает приложение//When the user has already clicked the button the app will minimized
        private void HideButton_MouseDown(object sender,MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }

        private void LogoSimplex_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}