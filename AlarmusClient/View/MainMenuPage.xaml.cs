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

namespace AlarmusClient.View
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        private void requestBtn_Click(object sender, RoutedEventArgs e)
        {
            RequestPage view = new RequestPage(this);
            NavigationService.Navigate(view);
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            AsyncClient.Disconnect();
            AutorizationPage view = new AutorizationPage();
            NavigationService.Navigate(view);
        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsPage view = new SettingsPage(this);
            NavigationService.Navigate(view);
        }
    }
}
