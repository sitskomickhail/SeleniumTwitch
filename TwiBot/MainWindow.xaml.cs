using System.Collections.Generic;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using Microsoft.Win32;
using TwiBot.Model.Repository;
using HttpWebRequestWrapper;
using Selenium.WebDriver.WaitExtensions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Net;
using System.IO;
using TwiBot.Model;
using OpenQA.Selenium.Firefox;

namespace TwiBot
{
    public partial class MainWindow : Window
    {
        private const string twitchStr = "https://www.twitch.tv/";
        private const string defPath = "E:\\audiofolder";

        private const string _urlPattern = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*";
        private List<IWebDriver> _driver;
        private List<Thread> _threads;
        //private FirefoxProfile fProf;
        private ChromeOptions options;
        UserRepository _users;
        //string pathToCurrentUserProfiles = Environment.ExpandEnvironmentVariables("%APPDATA%") + @"\Mozilla\Firefox\Profiles"; // Path to profile
        //string[] pathsToProfiles = Directory.GetDirectories(@"C:\Users\st\AppData\Roaming\Mozilla\Firefox\Profiles", "*.default", SearchOption.TopDirectoryOnly);

        public MainWindow()
        {
            InitializeComponent();
            options = new ChromeOptions();
            options.AddArgument("--safebrowsing-disable-download-protection");
            options.AddUserProfilePreference("download.default_directory", @"E:\audiofolder");
            //options.AddUserProfilePreference("intl.accept_languages", "en");
            //options.AddUserProfilePreference("disable-popup-blocking", "true");
            //options.AddArgument("headless");

            //fProf = new FirefoxProfile("C:\Users\st\AppData\Roaming\Mozilla\Firefox\Profiles\nydel6wd.recapUser");
            tbUrl.Text = twitchStr;
            _users = new UserRepository();
            _driver = new List<IWebDriver>();
            _threads = new List<Thread>();
        }

        private void btnStart_ClickTest(object sender, RoutedEventArgs e)
        {
            //    _driver.Add(new ChromeDriver(options));
            //    _driver[0].Manage().Window.Maximize();
            //    _driver[0].Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));

            GetCode(_driver[0]);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Regex regEx = new Regex(_urlPattern);
            string url = tbUrl.Text;
            if (!String.IsNullOrWhiteSpace(url) && regEx.IsMatch(url))
            {
                int i = 0;
                foreach (var user in _users.GetUser)
                {
                    //FirefoxProfile profile = new FirefoxProfile(pathsToProfiles[0]);
                    //profile.SetPreference("browser.tabs.loadInBackground", false); // set preferences you need
                    //_driver.Add(new FirefoxDriver(new FirefoxBinary(), profile, new TimeSpan(3)));
                    //_driver.Add(new FirefoxDriver(fProf));
                    _driver.Add(new ChromeDriver(options));

                    _driver[i].Manage().Window.Maximize();
                    _driver[i].Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                    Actions builder = new Actions(_driver[i]);
                    IWebElement audButton = null;
                    IWebElement downloadAudButton = null;

                    _driver[i].Navigate().GoToUrl(url);

                    _driver[i].FindElement(By.XPath("//*[@id=\"root\"]/div/div[2]/nav/div/div[5]/div/div[1]/button/span")).Click();
                    Thread.Sleep(5000);

                    IWebElement logIn = _driver[i].FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[1]/div/div[2]/input"));
                    logIn.Clear();
                    ReCaptchaDetect.Type_Like_Human(logIn, user.Login);
                    IWebElement passWrd = _driver[i].FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[2]/div/div[1]/div[2]/div[1]/input"));
                    passWrd.Clear();
                    ReCaptchaDetect.Type_Like_Human(passWrd, user.Password);
                    _driver[i].FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[3]/button/span")).Click();

                    Thread.Sleep(5301);
                    _driver[i].SwitchTo().Frame(_driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                    _driver[i].FindElement(By.ClassName("recaptcha-checkbox-checkmark")).Click();


                    Thread.Sleep(5102);

                    //image frame
                    //AudioButton find/click
                    try
                    {
                        _driver[i].SwitchTo().DefaultContent();
                        _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[9]);
                        _driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-audio-button\"]")).Click();
                    }
                    catch (Exception)
                    {
                        for (int j = 1; j < 10; j++)
                        {
                            bool check = false;
                            _driver[i].SwitchTo().DefaultContent();
                            try
                            {
                                _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[j]);
                                audButton = _driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-audio-button\"]"));
                                audButton.Click();
                                check = true;
                            }
                            catch (Exception) { }
                            if (check)
                                break;
                        }
                    }

                    Thread.Sleep(130000);
                    _driver[i].SwitchTo().DefaultContent();
                    _driver[i].SwitchTo().Frame(_driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                    _driver[i].FindElement(By.ClassName("recaptcha-checkbox-checkmark")).Click();
                    Thread.Sleep(1500);

                    //DownloadAudioButton click
                    try
                    {
                        _driver[i].SwitchTo().DefaultContent();
                        _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[9]);
                        _driver[i].FindElement(By.ClassName("rc-audiochallenge-tdownload-link")).Click();
                    }
                    catch (Exception)
                    {
                        for (int j = 1; j < 10; j++)
                        {
                            bool check = false;
                            _driver[i].SwitchTo().DefaultContent();

                            try
                            {
                                _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[j]);
                                downloadAudButton = _driver[i].FindElement(By.ClassName("rc-audiochallenge-tdownload-link"));
                                downloadAudButton.Click();
                                check = true;
                            }
                            catch (Exception) { }
                            if (check)
                                break;
                        }
                    }


                    List<string> tabs = new List<string>(_driver[i].WindowHandles);
                    _driver[i].SwitchTo().Window(tabs[1]);
                    Thread.Sleep(1000);

                    DownloadAudio(_driver[i].Url, _driver[i]); //amoyshare
                    GetCode(_driver[i]); //realspeaker

                    _driver[i].SwitchTo().Window(tabs[0]);

                    _driver[i].SwitchTo().DefaultContent();
                    _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[9]);

                    IWebElement audioTB = _driver[i].FindElement(By.Id("audio-response"));
                    audioTB.Clear();
                    ReCaptchaDetect.Type_Like_Human(audioTB, Clipboard.GetText());

                    Thread.Sleep(4000);
                    _driver[i].FindElement(By.Id("recaptcha-verify-button")).Click();

                    Thread.Sleep(3000);
                    _driver[i].SwitchTo().DefaultContent();
                    _driver[i].SwitchTo().Frame(_driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                    _driver[i].FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/div[3]/div/button/span")).Click();
                    //Screenshot ss = ((ITakesScreenshot)_driver[i]).GetScreenshot();
                    //ss.SaveAsFile(@"D:\1\TEST\" + i.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    i++;

                    DirectoryInfo di = new DirectoryInfo(defPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
            }
            else
                MessageBox.Show("Uncorrect URL-code",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void btnGenerateKey_Click(object sender, RoutedEventArgs e)
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
            foreach (IWebDriver item in _driver)
            {
                item.Quit();
            }
        }


        private void btnHelp_Mark_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bot info:/nlogin:password:mail");
        }


        private void GetCode(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://realspeaker.net/");
            Thread.Sleep(5000);
            driver.FindElement(By.XPath("//*[@id=\"app\"]/main/div/div/div/div/div[2]/div/button/div")).Click();
            Thread.Sleep(2500);
            IWebElement uploadAudio = driver.FindElement(By.XPath("//*[@id=\"uploader\"]/div/div/div[2]/input"));
            uploadAudio.SendKeys(defPath + "\\audio-audio.mp3");
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//*[@id=\"app\"]/main/div/div/div/div/div[6]/div/button/div")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//*[@id=\"app\"]/main/div/div[1]/div/div/div/ul/span/li[1]/div/div[2]/div/button[2]/div")).Click();
            Thread.Sleep(9000);

            try { driver.FindElement(By.ClassName("btn__content")).Click(); }
            catch { driver.FindElement(By.XPath("//*[@id=\"app\"]/main/div/div[1]/div/div/div/ul/span/li[1]/div/div[2]/div/button[1]/div")).Click(); }

            Thread.Sleep(3000);
            driver.FindElement(By.XPath("//*[@id=\"app\"]/main/div/div[1]/div/div/span/div/nav/div/button/div")).Click();
        }


        private void DownloadAudio(string url, IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://www.amoyshare.com/free-mp3-finder/");
            IWebElement urlPatse = driver.FindElement(By.XPath("//*[@id=\"commonFinderSearch\"]/div[1]/input"));
            urlPatse.Clear(); urlPatse.SendKeys(url);
            driver.FindElement(By.XPath("//*[@id=\"commonFinderSearch\"]/div[1]/div[1]")).Click();
            Thread.Sleep(6500);
            driver.FindElement(By.XPath("//*[@id=\"commonFinder\"]/div[7]/div/div[1]/div/div[3]/div[2]")).Click();
            Thread.Sleep(4000);
            driver.FindElement(By.XPath("//*[@id=\"commonFinder\"]/div[7]/div/div[3]/div[3]/div/ul/li[2]/a/div[2]")).Click();
            Thread.Sleep(8000);
        }

        private void MPtoWavAndRecover(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Model.SpeechRec.ConvertMp3ToWavAndRecover(@"E:\folder\audio-audio.mp3", @"E:\folderWav"));
        }
    }
}