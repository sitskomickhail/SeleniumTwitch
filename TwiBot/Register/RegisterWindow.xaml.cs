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
using System.Windows.Shapes;
using TwiBot.Data;
using TwiBot.Data.UIDATA;

namespace TwiBot.Register
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void RegisterWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!DBWorker.SetLicenseKey(tbKey.Text))
            {
                MessageBox.Show("Uncorrect license key");
                return;
            }
            Crypting.CryptDataInFile(tbKey.Text);
            this.Visibility = Visibility.Hidden;
            Application.Current.Windows[0].Visibility = Visibility.Visible;
        }

        private void tbKey_GotFocus(object sender, RoutedEventArgs e)
        {
            tbKey.Clear();
        }

        private void tbKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbKey.Text.ToString()))
                tbKey.Text = "Input license key here";
        }
    }
}
