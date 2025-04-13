using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Транспортные_задачи
{
    /// <summary>
    /// Логика взаимодействия для MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        private void NorthWestButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NorthWestPage());
        }

        private void MinElementsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MinElementsPage());
        }

        private void SimplexButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SimplexPage());
        }
    }

}

