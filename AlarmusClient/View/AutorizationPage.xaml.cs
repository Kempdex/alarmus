using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace AlarmusClient.View
{
    /// <summary>
    /// Interaction logic for AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        Thread clientThread;

        public AutorizationPage()
        {
            InitializeComponent();
            addressBox.Text = "192.168.1.46";

            Alarmus.Log.SetLogFileName("ClientLog.txt");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            loginBtn.IsEnabled = false;
            Alarmus.AutorizationMessage msg = new Alarmus.AutorizationMessage(loginBox.Text, passwordBox.Password);
            
            AsyncClient.Connect(addressBox.Text, 8888); //TODO: Заменить адрес из текст бокса на параметр
            clientThread = new Thread(() => AsyncAutorization(msg));
            clientThread.Start();
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void changeView()
        {
            MainMenuPage view = new MainMenuPage();
            NavigationService.Navigate(view);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void setButtonEnable(bool state)
        {
            loginBtn.IsEnabled = state;
        }

        private void AsyncAutorization(Alarmus.AutorizationMessage msg)
        {
            AsyncClient.SetToNullServerResponse();
            AsyncClient.SendMessage(msg);

            /* Серебрянная пуля.
             * Поток будет ждать, пока данные полностью дойдут
             * Если избегать этого, то, недождавшись ответа от сервера, сработает ветка SR_NULL
             * И это происходит достаточно часто.
             */
            Thread.Sleep(500);

            if (AsyncClient.GetServerResponse() == Alarmus.ServerResponse.SR_AUTORIZATION_SUCCESS)
            {
                /*
                 * Вызываем диспетчер, который будет вызывать методы связанные с UI в thread safe режиме
                 */
                Dispatcher.Invoke(new System.Action(() => setButtonEnable(true)));
                Dispatcher.Invoke(new System.Action(() => changeView()));
                return;
            }
            else if (AsyncClient.GetServerResponse() == Alarmus.ServerResponse.SR_AUTORIZATION_FAILED)
            {
                MessageBox.Show("Невозможно авторизироваться. Проверьте логин и пароль");
            }
            else if(AsyncClient.GetServerResponse() == Alarmus.ServerResponse.SR_NULL)
            {
                MessageBox.Show("Невозможно авторизироваться. Попробуйте позже");
                Alarmus.Log.Warning("Этого не должно было произойти...");
            }

            Dispatcher.Invoke(new System.Action(() => setButtonEnable(true)));
        }

    }
}
