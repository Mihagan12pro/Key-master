using Key_master.WPF.ViewModels;
using System.Windows;

namespace Key_master.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow(double width,double length)
        {
            InitializeComponent();

            DataContext = new EditWindowViewModel(width,length);
        }
    }
}
