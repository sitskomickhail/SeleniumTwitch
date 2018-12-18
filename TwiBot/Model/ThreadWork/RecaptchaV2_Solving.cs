using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TwiBot.Model.Repository;

namespace TwiBot.Model.ThreadWork
{
    public class RecaptchaV2_Solving
    {
        private ChromeOptions options;
        private int _framePos;
        private IWebDriver _driver;

        private void SetDriverOptions()
        {
            options = new ChromeOptions();
            options.AddArgument("--safebrowsing-disable-download-protection");
            //options.AddUserProfilePreference("download.default_directory", @"E:\audiofolder");
            //options.AddUserProfilePreference("intl.accept_languages", "en");
            //options.AddUserProfilePreference("disable-popup-blocking", "true");
            //options.AddArgument("headless");
        }

        private void Type_Like_Human(IWebElement element, string str)
        {
            foreach (char c in str)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(Randomer.Next(150, 450));
            }
        }

        public void GoTo_Twitch(string url, string userLogin, string userPassword)
        {
            #region DRIVER_CONFIGURATION
            SetDriverOptions();
            _driver = new ChromeDriver(options);
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
            #endregion

            Actions builder = new Actions(_driver);
            IWebElement audButton = null;
            IWebElement downloadAudButton = null;

            _driver.Navigate().GoToUrl(url);

            #region LOGIN_NAVIGATE
            _driver.FindElement(By.XPath("//*[@id=\"root\"]/div/div[2]/nav/div/div[5]/div/div[1]/button/span")).Click();
            Thread.Sleep(5000);

            IWebElement logIn = _driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[1]/div/div[2]/input"));
            logIn.Clear();
            Type_Like_Human(logIn, userLogin);
            IWebElement passWrd = _driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[2]/div/div[1]/div[2]/div[1]/input"));
            passWrd.Clear();
            Type_Like_Human(passWrd, userPassword);
            _driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/form/div/div[3]/button/span")).Click();

            Thread.Sleep(5301);
            _driver.SwitchTo().Frame(_driver.FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
            _driver.FindElement(By.ClassName("recaptcha-checkbox-checkmark")).Click();
            #endregion

            Thread.Sleep(5102);

            #region AUDIO_BUTTON__CLICK
            try
            {
                _driver.SwitchTo().DefaultContent();
                _driver.SwitchTo().Frame(_driver.FindElements(By.TagName("iframe"))[9]);
                _framePos = 9;
                _driver.FindElement(By.XPath("//*[@id=\"recaptcha-audio-button\"]")).Click();
            }
            catch (Exception)
            {
                for (int j = 0; j < 10; j++)
                {
                    bool check = false;
                    _driver.SwitchTo().DefaultContent();
                    try
                    {
                        _driver.SwitchTo().Frame(_driver.FindElements(By.TagName("iframe"))[j]);
                        audButton = _driver.FindElement(By.XPath("//*[@id=\"recaptcha-audio-button\"]"));
                        audButton.Click();
                        _framePos = j;
                        check = true;
                    }
                    catch (Exception) { }
                    if (check)
                        break;
                }
            }
            #endregion
            bool checkLinkTab = false;
            try { _driver.FindElement(By.ClassName("rc-audiochallenge-tdownload-link")).Click(); checkLinkTab = true; }
            catch (Exception) { Thread.Sleep(130000); }

            if (!checkLinkTab)
            {
                #region GO_SOLVE!
                while (true)
                {
                    try
                    {
                        _driver.SwitchTo().Frame(_driver.FindElements(By.TagName("iframe"))[_framePos]);
                        Thread.Sleep(5000);
                        continue;
                    }
                    catch
                    {
                        _driver.SwitchTo().DefaultContent();
                        _driver.SwitchTo().Frame(_driver.FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                        _driver.FindElement(By.ClassName("recaptcha-checkbox-checkmark")).Click();
                        Thread.Sleep(1500);
                        break;
                    }
                }
                #endregion

                #region AUDIO_BUTTON__DOWNLOAD
                try
                {
                    _driver.SwitchTo().DefaultContent();
                    _driver.SwitchTo().Frame(_driver.FindElements(By.TagName("iframe"))[9]);
                    _driver.FindElement(By.ClassName("rc-audiochallenge-tdownload-link")).Click();
                    _framePos = 9;
                }
                catch (Exception)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        bool check = false;
                        _driver.SwitchTo().DefaultContent();

                        try
                        {
                            _driver.SwitchTo().Frame(_driver.FindElements(By.TagName("iframe"))[j]);
                            downloadAudButton = _driver.FindElement(By.ClassName("rc-audiochallenge-tdownload-link"));
                            downloadAudButton.Click();
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

            List<string> tabs = new List<string>(_driver.WindowHandles);
            _driver.SwitchTo().Window(tabs[1]);

            #region DOWNLOAD/GET_CODE
            DownloadRequest(_driver.Url);
            GetCode(_driver);
            #endregion

            _driver.SwitchTo().Window(tabs[0]);

            #region CREATE_ANSWER
            _driver.SwitchTo().DefaultContent();
            _driver.SwitchTo().Frame(_driver.FindElements(By.TagName("iframe"))[_framePos]);

            IWebElement audioTB = _driver.FindElement(By.Id("audio-response"));
            audioTB.Clear();
            Type_Like_Human(audioTB, Clipboard.GetText());

            int locationX = _driver.FindElement(By.Id("recaptcha-verify-button")).Location.X;
            int locationY = _driver.FindElement(By.Id("recaptcha-verify-button")).Location.Y;
            builder.MoveToElement(_driver.FindElement(By.Id("recaptcha-verify-button")), locationX + 7, locationY + 7).Click();

            Thread.Sleep(2500);
            SOLVE_CAPTCHA_SECOND_TIME(_driver);

            Thread.Sleep(2500);
            _driver.SwitchTo().DefaultContent();
            _driver.FindElement(By.XPath("/html/body/div[2]/div/div/div/div[1]/div/div/div[3]/div/button")).Click(); MessageBox.Show("1");
            #endregion
            //Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();
            //ss.SaveAsFile(@"D:\1\TEST\" + i.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
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
            Thread.Sleep(130000);

            #region GO_SOLVE!
            while (true)
            {
                try
                {
                    driver.SwitchTo().Frame(driver.FindElements(By.TagName("iframe"))[_framePos]);
                    Thread.Sleep(5000);
                    continue;
                }
                catch
                {
                    driver.SwitchTo().DefaultContent();
                    driver.SwitchTo().Frame(driver.FindElement(By.XPath("//*[@id=\"recaptcha-element-container\"]/div/div/iframe")));
                    driver.FindElement(By.ClassName("recaptcha-checkbox-checkmark")).Click();
                    Thread.Sleep(1500);
                    break;
                }
            }
            #endregion

            #region AUDIO_BUTTON__DOWNLOAD
            try
            {
                driver.SwitchTo().DefaultContent();
                driver.SwitchTo().Frame(driver.FindElements(By.TagName("iframe"))[9]);
                driver.FindElement(By.ClassName("rc-audiochallenge-tdownload-link")).Click();
                _framePos = 9;
            }
            catch (Exception)
            {
                for (int j = 0; j < 10; j++)
                {
                    bool check = false;
                    driver.SwitchTo().DefaultContent();

                    try
                    {
                        driver.SwitchTo().Frame(driver.FindElements(By.TagName("iframe"))[j]);
                        driver.FindElement(By.ClassName("rc-audiochallenge-tdownload-link")).Click();
                        _framePos = j;
                        check = true;
                    }
                    catch (Exception) { }
                    if (check)
                        break;
                }
            }
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

            IWebElement audioTB = driver.FindElement(By.Id("audio-response"));
            audioTB.Clear();
            Type_Like_Human(audioTB, Clipboard.GetText());

            Actions builder = new Actions(driver);
            int locationX = driver.FindElement(By.Id("recaptcha-verify-button")).Location.X;
            int locationY = driver.FindElement(By.Id("recaptcha-verify-button")).Location.Y;
            builder.MoveToElement(driver.FindElement(By.Id("recaptcha-verify-button")), locationX + 7, locationY + 7).Click();
            #endregion
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

        public void CloseDriver()
        {
            if (_driver != null)
                _driver.Close();
        }
    }
}
