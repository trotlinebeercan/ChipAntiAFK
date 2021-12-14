using System.Reflection;
using System.Windows;

namespace ChipAntiAFK
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = "ChipAA v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
        }
    }
}
