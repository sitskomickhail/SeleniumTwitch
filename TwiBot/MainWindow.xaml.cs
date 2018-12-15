using System.Collections.Generic;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;
using TwiBot.Model.Repository;
using OpenQA.Selenium.Interactions;
using System.Net;
using System.IO;
using TwiBot.Model;

namespace TwiBot
{
    public partial class MainWindow : Window
    {
        private const string twitchStr = "https://www.twitch.tv/";
        private const string defPath = "E:\\audiofolder";
        private const string _urlPattern = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*";

        private int _framePos;
        private List<IWebDriver> _driver;
        private List<Thread> _threads;
        //private FirefoxProfile fProf;
        private ChromeOptions options;
        UserRepository _users;

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
                    #region DRIVER_CONFIGURATION
                    _driver.Add(new ChromeDriver(options));
                    _driver[i].Manage().Window.Maximize();
                    _driver[i].Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    #endregion

                    Actions builder = new Actions(_driver[i]);
                    IWebElement audButton = null;
                    IWebElement downloadAudButton = null;

                    _driver[i].Navigate().GoToUrl(url);

                    #region LOGIN_NAVIGATE
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
                    #endregion

                    Thread.Sleep(5102);
                    
                    #region AUDIO_BUTTON__CLICK
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
                                _framePos = j;
                                audButton.Click();
                                check = true;
                            }
                            catch (Exception) { }
                            if (check)
                                break;
                        }
                    }
                    #endregion

                    Thread.Sleep(130000);
                    _driver[i].SwitchTo().DefaultContent();
                    _driver[i].SwitchTo().Frame(_driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                    _driver[i].FindElement(By.ClassName("recaptcha-checkbox-checkmark")).Click();
                    Thread.Sleep(1500);

                    #region AUDIO_BUTTON__DOWNLOAD
                    try
                    {
                        _driver[i].SwitchTo().DefaultContent();
                        _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[_framePos]);
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
                    #endregion
                    
                    List<string> tabs = new List<string>(_driver[i].WindowHandles);
                    _driver[i].SwitchTo().Window(tabs[1]);

                    #region DOWNLOAD/GET_CODE
                    DownloadRequest(_driver[i].Url);
                    GetCode(_driver[i]);
                    #endregion
                    
                    _driver[i].SwitchTo().Window(tabs[0]);

                    #region CREATE_ANSWER
                    _driver[i].SwitchTo().DefaultContent();
                    _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[_framePos]);

                    IWebElement audioTB = _driver[i].FindElement(By.Id("audio-response"));
                    audioTB.Clear();
                    ReCaptchaDetect.Type_Like_Human(audioTB, Clipboard.GetText());

                    int locationX = _driver[i].FindElement(By.Id("recaptcha-verify-button")).Location.X;
                    int locationY = _driver[i].FindElement(By.Id("recaptcha-verify-button")).Location.Y;
                    builder.MoveToElement(_driver[i].FindElement(By.Id("recaptcha-verify-button")), locationX, locationY).Click();

                    Thread.Sleep(4000);
                    _driver[i].FindElement(By.Id("recaptcha-verify-button")).Click();

                    Thread.Sleep(3000);
                    _driver[i].SwitchTo().DefaultContent();
                    _driver[i].SwitchTo().Frame(_driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                    _driver[i].FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/div[3]/div/button/span")).Click();
                    #endregion
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
            uploadAudio.SendKeys($@"{Environment.CurrentDirectory}\audioRecaptcha.mp3");
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
        
        private void DownloadRequest(string url)
        {
            HttpWebRequest httpRequest = (HttpWebRequest) WebRequest.Create(url);
            httpRequest.Method = WebRequestMethods.Http.Get;
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream httpResponseStream = httpResponse.GetResponseStream();

            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;

            FileStream fileStream = File.Create("audioRecaptcha.mp3");
            while ((bytesRead = httpResponseStream.Read(buffer, 0, bufferSize)) != 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
        }

        private void btnGetCodeTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Model.SpeechRec.ConvertMp3ToWavAndRecover($@"{Environment.CurrentDirectory}\request.mp3", @"E:\folderWav"));
        }

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {
            DownloadRequest(tbUrl.Text);
        }
    }
}