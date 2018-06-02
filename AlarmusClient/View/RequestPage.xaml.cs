using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private MainMenuPage parent;

        public RequestPage(MainMenuPage parent)
        {
            InitializeComponent();

            this.parent = parent;

            //Just for test
            troubleTypeBox.Items.Add("Test1");
            troubleTypeBox.Items.Add("Test2");
            troubleTypeBox.Items.Add("Test3");
            troubleTypeBox.Items.Add("Test4");
            troubleTypeBox.SelectedIndex = 0;
        }

        private void sendRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            Alarmus.RequestMessage request = new Alarmus.RequestMessage(troubleDataBox.Text, troubleTypeBox.SelectedItem.ToString());
            AsyncClient.SendMessage(request);
            sendRequestBtn.IsEnabled = false;
            /*
             * Самый простой способ подождать сеть.
             * Ждем, пока асинхронный клиент получит ответ от сервера
             */
            Thread.Sleep(500);

            //Проверяем ответ от сервера
            switch(AsyncClient.GetServerResponse())
            {
                case Alarmus.ServerResponse.SR_REQUEST_SUCCESS:
                    MessageBox.Show("Заявка успешно отправлена");
                    break;
                case Alarmus.ServerResponse.SR_REQUEST_FAILED:
                    MessageBox.Show("Заявка не была отправлена. Попробуйте позже");
                    break;
                case Alarmus.ServerResponse.SR_NULL:
                    MessageBox.Show("Заявка не была отправлена. Возможны неполадки с сетью");
                    Alarmus.Log.Warning("Ответ на заявку не успел дойти. Возможны неполадки с сетью или сервер был отсоединен");
                    break;
            }

            sendRequestBtn.IsEnabled = true;
        }

        private void backToMainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(parent);
        }
    }
}
