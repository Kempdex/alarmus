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

namespace AlarmusServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread serverThread;
        public MainWindow()
        {
            InitializeComponent();
            Alarmus.Log.WriteToComponent component = new Alarmus.Log.WriteToComponent(AddInfoToListBox);
            Alarmus.Log.LinkComponentMethod(AddInfoToListBox);

            DatabaseMaster.Initialize();
            Alarmus.RequestMessage msg = new Alarmus.RequestMessage("Test trouble Data", "Test trouble type");
           // DatabaseMaster.AddRequest(msg);
           // DatabaseMaster.AddRequest(msg);

            List<Alarmus.RequestMessage> list = DatabaseMaster.GetAllRequests();
            list.ForEach(x => Alarmus.Log.Debug(x.getTroubleType(), x.getTroubleData()));

            portBox.Text = "8888";
            countOfConnectionsBox.Text = "1000";
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            Int32 num;
            if(!Int32.TryParse(portBox.Text, out num))
            {
                MessageBox.Show("Введите корректное значение в поле 'Порт'", "Неверное значение");
                return;
            }
            if(!Int32.TryParse(countOfConnectionsBox.Text, out num))
            {
                MessageBox.Show("Введите корректное значение в поле 'Максимальное количество подключений'", "Неверное значение");
                return;
            }

            AsyncServer.InitializeServer(Convert.ToInt32(portBox.Text), Convert.ToInt32(countOfConnectionsBox.Text));
            serverThread = new Thread( () => AsyncServer.Start() );
            serverThread.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(serverThread != null)
            {
                serverThread.Abort();
            }
        }

        public void AddInfoToListBox(params object[] info)
        {
            string data = String.Empty;

            for(Int32 i = 0; i < info.Length; i++)
            {
                data += info[i] + " ";
            }

           // logList.Items.Add(data);
            Dispatcher.Invoke(new Action(() => logList.Items.Add(data)));
        }
    }
}
