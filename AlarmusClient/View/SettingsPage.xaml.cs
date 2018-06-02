using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private MainMenuPage parent;

        public SettingsPage(MainMenuPage parent)
        {            
            InitializeComponent();

            this.parent = parent;
            serverAddressBox.Text = Properties.Settings.Default.serverAddress;
            portBox.Text = Properties.Settings.Default.serverPort.ToString();
        }

        private void backToMainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(parent);
        }

        private void acceptSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            IPAddress serverAddress;
            Int32 serverPort;
            if(!IPAddress.TryParse(serverAddressBox.Text, out serverAddress))
            {
                MessageBox.Show("Неверный IP адрес");
                return;
            }
            if(!Int32.TryParse(portBox.Text, out serverPort))
            {
                MessageBox.Show("Неверный порт");
                return;
            }

            Properties.Settings.Default.serverAddress = serverAddressBox.Text;
            Properties.Settings.Default.serverPort = serverPort;
            Properties.Settings.Default.Save();
            MessageBox.Show("Настройки обновлены");
        }
    }
}
