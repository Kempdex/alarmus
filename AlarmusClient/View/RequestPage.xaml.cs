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
    /// Interaction logic for RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        public RequestPage()
        {
            InitializeComponent();

            troubleTypeBox.Items.Add("Test1");
            troubleTypeBox.Items.Add("Test2");
            troubleTypeBox.Items.Add("Test3");
            troubleTypeBox.Items.Add("Test4");
        }

        private void sendRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            Alarmus.RequestMessage request = new Alarmus.RequestMessage(troubleDataBox.Text, troubleTypeBox.SelectedItem.ToString());
            AsyncClient.Connect("192.168.1.46", 8888);
          //  MessageBox.Show(AsyncClient.isConnected.ToString());
            AsyncClient.SendMessage(request);
           
        }
    }
}
