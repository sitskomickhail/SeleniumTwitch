using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwiBot.Model
{
    public static class ReCaptchaDetect
    {
        public static void Type_Like_Human(IWebElement element, string str)
        {
            foreach (char c in str)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(Randomer.Next(250, 750));
            }
        }
    }
}
