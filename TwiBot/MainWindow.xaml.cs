using System.Collections.Generic;
using System.Windows;
using OpenQA.Selenium;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;
using TwiBot.Model.Repository;
using System.Net;
using System.IO;
using TwiBot.Model.ThreadWork;
//test
using TwiBot.TestCompiles;

namespace TwiBot
{
    public partial class MainWindow : Window
    {
        private UserRepository _users;
        private const string twitchStr = "https://www.twitch.tv/";
        private const string _urlPattern = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*";
        private ThreadDriver _thread;

        public MainWindow()
        {
            InitializeComponent();
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
                //_thread.StartDriver_Work(url);
                _thread.TestStartDriver_Work(url);
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
        }

        private void btnHelp_Mark_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bot info:\nlogin:password:mail");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}