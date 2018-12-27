using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TwiBot.Model.Repository;

namespace TwiBot.Model.ThreadWork
{
    public class RecaptchaSolve
    {
        private List<IWebDriver> _driver;
        private ChromeOptions options;
        private UserRepository _users;
        private int _framePos;
        private int _implicitTime;

        public RecaptchaSolve()
        {
            _driver = new List<IWebDriver>();
            _users = new UserRepository();
        }

        private void SetDriverOptions()
        {
            options = new ChromeOptions();
            options.AddArgument("--safebrowsing-disable-download-protection");
            //options.AddArgument("headless");
        }

        private void Type_Like_Human(IWebElement element, string str)
        {
            foreach (char c in str)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(Randomer.Next(100, 400));
            }
        }

        public void GoTo_Twitch(string url)
        {
            int i = 0;
            _implicitTime = 1;
            foreach (var user in _users.GetUser)
            {
                #region DRIVER_CONFIGURATION
                SetDriverOptions();
                _driver.Add(new ChromeDriver(options));
                _driver[i].Manage().Window.Maximize();
                _driver[i].Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
                #endregion
                Actions builder = new Actions(_driver[i]);

                _driver[i].Navigate().GoToUrl(url);

                #region LOGIN_NAVIGATE
                DriverWaitExtensions.FindElement(_driver[i], By.XPath("//*[@id=\"root\"]/div/div[2]/nav/div/div[5]/div/div[1]/button/span"), 10 * _implicitTime).Click();

                IWebElement logIn = DriverWaitExtensions.FindElement(_driver[i], By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[1]/div/div[2]/input"), 2 * _implicitTime);
                logIn.Clear();
                Type_Like_Human(logIn, user.Login);
                IWebElement passWrd = DriverWaitExtensions.FindElement(_driver[i], By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[2]/div/div[1]/div[2]/div[1]/input"), 2 * _implicitTime);
                passWrd.Clear();
                Type_Like_Human(passWrd, user.Password);
                DriverWaitExtensions.FindElement(_driver[i], By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[3]/button/span"), 2 * _implicitTime).Click();

                _driver[i].SwitchTo().Frame(DriverWaitExtensions.FindElement(_driver[i], By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe"), 3 * _implicitTime));
                DriverWaitExtensions.FindElement(_driver[i], By.ClassName("recaptcha-checkbox-checkmark"), 7 * _implicitTime).Click();
                #endregion

                #region AUDIO_BUTTON__CLICK
                IWebElement btn;
                _driver[i].Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                Thread.Sleep(3000);
                try
                {
                    _driver[i].SwitchTo().DefaultContent();
                    _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[9]);
                    btn = _driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-audio-button\"]"));
                    Thread.Sleep(3000);
                    btn.Click();
                    _framePos = 9;
                }
                catch (Exception)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        bool check = false;
                        _driver[i].SwitchTo().DefaultContent();
                        try
                        {
                            _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[j]);
                            btn = _driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-audio-button\"]"));
                            Thread.Sleep(1000);
                            btn.Click();
                            _framePos = j;
                            check = true;
                        }
                        catch (Exception) { }
                        if (check)
                            break;
                    }
                }
                #endregion
                try { _driver[i].FindElement(By.ClassName("rc-audiochallenge-tdownload-link")).Click(); }
                catch
                {
                    #region GO_SOLVE!
                    _driver[i].Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(140);
                    Thread.Sleep(127000);
                    while (true)
                    {
                        try
                        {
                            _driver[i].SwitchTo().DefaultContent();
                            _driver[i].SwitchTo().Frame(_driver[i].FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                            IWebElement captchaClick = _driver[i].FindElement(By.ClassName("recaptcha-checkbox-checkmark"));
                            captchaClick.Click();
                            break;
                        }
                        catch { Thread.Sleep(5025); }
                    }
                    #endregion
                    //Actions act = new Actions(_driver[i]); // ACTION MOVE TO DOWNLOAD AUDIO BUTTON
                    Thread.Sleep(5000);
                    _driver[i].Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                    #region AUDIO_BUTTON__DOWNLOAD
                    try
                    {
                        _driver[i].SwitchTo().DefaultContent();
                        _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[_framePos]);
                        btn = _driver[i].FindElement(By.ClassName("rc-audiochallenge-tdownload-link"));
                        btn.Click();
                    }
                    catch (Exception)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            bool check = false;
                            _driver[i].SwitchTo().DefaultContent();
                            try
                            {
                                _driver[i].SwitchTo().Frame(_driver[i].FindElements(By.TagName("iframe"))[j]);
                                btn = _driver[i].FindElement(By.ClassName("rc-audiochallenge-tdownload-link"));
                                btn.Click();
                                _framePos = j;
                                check = true;
                            }
                            catch (Exception) { }
                            if (check)
                                break;
                        }
                    }
                    #endregion
                }
                _driver[i].Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
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

                IWebElement audioTB = DriverWaitExtensions.FindElement(_driver[i], By.Id("audio-response"), 2 * _implicitTime);
                audioTB.Clear();
                Type_Like_Human(audioTB, Clipboard.GetText());

                try
                {
                    _driver[i].FindElement(By.Id("recaptcha-verify-button")).Click();
                    try
                    {
                        _driver[i].SwitchTo().DefaultContent();
                        _driver[i].FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/div[3]/div/button")).Click();
                    }
                    catch { SOLVE_CAPTCHA_SECOND_TIME(_driver[i]); }
                }
                catch { SOLVE_CAPTCHA_SECOND_TIME(_driver[i]); }

                _driver[i].SwitchTo().DefaultContent();
                DriverWaitExtensions.FindElement(_driver[i], By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/div[3]/div/button"), 5 * _implicitTime).Click();
                #endregion
                i++;
                if ((double)i / _implicitTime == 3)
                    _implicitTime++;
                //Screenshot ss = ((ITakesScreenshot)_driver[i]).GetScreenshot();
                //ss.SaveAsFile(@"D:\1\TEST\" + i.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void DownloadRequest(string url)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
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

        private void SOLVE_CAPTCHA_SECOND_TIME(IWebDriver driver)
        {
            Thread.Sleep(127000);
            while (true)
            {
                try
                {
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(driver.FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                    driver.FindElement(By.ClassName("recaptcha-checkbox-checkmark")).Click();

                    break;
                }
                catch { }
            }

            #region AUDIO_BUTTON__DOWNLOAD
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(driver.FindElements(By.TagName("iframe"))[_framePos]);
            DriverWaitExtensions.FindElement(driver, By.ClassName("rc-audiochallenge-tdownload-link"), 1 * _implicitTime).Click();
            #endregion

            List<string> tabs = new List<string>(driver.WindowHandles);
            driver.SwitchTo().Window(tabs[2]);

            #region DOWNLOAD/GET_CODE
            DownloadRequest(driver.Url);
            GetCode(driver);
            #endregion

            driver.SwitchTo().Window(tabs[0]);

            #region CREATE_ANSWER
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(driver.FindElements(By.TagName("iframe"))[_framePos]);

            IWebElement audioTB = DriverWaitExtensions.FindElement(driver, By.Id("audio-response"), 2 * _implicitTime);
            audioTB.Clear();
            Type_Like_Human(audioTB, Clipboard.GetText());

            DriverWaitExtensions.FindElement(driver, By.Id("recaptcha-verify-button"), 2 * _implicitTime).Click();
            #endregion
        }

        private void GetCode(IWebDriver driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://realspeaker.net/");

            DriverWaitExtensions.FindElement(driver, By.XPath("//*[@id=\"app\"]/main/div/div/div/div/div[2]/div/button/div"), 7 * _implicitTime).Click();

            IWebElement uploadAudio = DriverWaitExtensions.FindElement(driver, By.XPath("//*[@id=\"uploader\"]/div/div/div[2]/input"), 5 * _implicitTime);
            uploadAudio.SendKeys($@"{Environment.CurrentDirectory}\audioRecaptcha.mp3");

            DriverWaitExtensions.FindElement(driver, By.XPath("//*[@id=\"app\"]/main/div/div/div/div/div[6]/div/button/div"), 10 * _implicitTime).Click();

            DriverWaitExtensions.FindElement(driver, By.XPath("//*[@id=\"app\"]/main/div/div[1]/div/div/div/ul/span/li[1]/div/div[2]/div/button[2]/div"), 5 * _implicitTime).Click();


            //try { DriverWaitExtensions.FindElement(driver, By.ClassName("btn__content"), 15).Click(); }
            //catch { DriverWaitExtensions.FindElement(driver, By.XPath("//*[@id=\"app\"]/main/div/div[1]/div/div/div/ul/span/li[1]/div/div[2]/div/button[1]/div"), 15).Click(); }
            DriverWaitExtensions.FindElement(driver, By.XPath("//*[@id=\"app\"]/main/div/div[1]/div/div/div/ul/span/li[1]/div/div[2]/div/button[1]/div"), 15 * _implicitTime).Click();

            DriverWaitExtensions.FindElement(driver, By.XPath("//*[@id=\"app\"]/main/div/div[1]/div/div/span/div/nav/div/button/div"), 5 * _implicitTime).Click();
        }

        public void CloseDriver()
        {
            if (_driver != null)
                foreach (var driver in _driver)
                {
                    driver.Close();
                }
        }
    }
}