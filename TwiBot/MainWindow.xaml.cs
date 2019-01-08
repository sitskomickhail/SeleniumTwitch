using System.Windows;
using System;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using TwiBot.Model.Repository;
using TwiBot.Model.ThreadWork;
using TwiBot.TestCompiles;
using TwiBot.Register;

namespace TwiBot
{
    public partial class MainWindow : Window
    {
        private UserRepository _users;
        private const string twitchStr = "https://www.twitch.tv/";
        private const string _urlPattern = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*";
        private ThreadDriver _thread;
        RegisterWindow regWin;
        private bool TEST = false;

        public MainWindow()
        {
            InitializeComponent();
            //if (!DBWorker.IsLicenseKey_Exist())
            if (TEST)
            {
                regWin = new RegisterWindow();
                regWin.Show();
                regWin.Activate();
                regWin.Topmost = true;
                this.Visibility = Visibility.Hidden;
            }
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            _users = new UserRepository();
            _thread = new ThreadDriver();
            tbUrl.Text = twitchStr;
        }

        private void btnStart_ClickTEST(object sender, RoutedEventArgs e)
        {
            RecaptchaTest recap = new RecaptchaTest();
            recap.GoTo_Twitch();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Regex regEx = new Regex(_urlPattern);
            string url = tbUrl.Text;
            if (!String.IsNullOrWhiteSpace(url) && regEx.IsMatch(url))
            {
                int count = 0;
                Int32.TryParse(tbCount.Text, out count);
                _thread.StartDriver_Work(url, count);
            }
            else
                MessageBox.Show("Uncorrect URL-code",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void btnAddBotInfo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog().Value)
            {
                string txt = file.FileName;
                _users.AddUsers(txt);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _thread.Kill_Threads();
            Application.Current.Shutdown();
        }

        private void btnHelp_Mark_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bot info:\nlogin:password:mail");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _thread.Kill_Threads();
        }
    }
}