using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace TwiBot.Model
{
    public static class DriverWaitExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by) ?? null);
                /*{
                    /*try
                    {
                        drv.FindElement(by);
                    }
                    catch
                    {
                        throw new NoSuchElementException("No such element");
                    }
                });*/
            }
            return driver.FindElement(by);
        }
    }
}
