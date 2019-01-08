using System;
using System.Threading;
using TwiBot.Model.Repository;

namespace TwiBot.Model.ThreadWork
{
    public class ThreadDriver
    {
        private UserRepository _users;
        private Thread _testThread;
        private RecaptchaSolve _recaptcha;

        public ThreadDriver()
        {
            _users = new UserRepository();
            _recaptcha = new RecaptchaSolve();
        }

        [STAThread]
        public void StartDriver_Work(string url, int count)
        {
            _testThread = new Thread(() => _recaptcha.GoTo_Twitch(url, count));
            _testThread.SetApartmentState(ApartmentState.STA);
            _testThread.Start();
        }

        public void Kill_Threads()
        {
            _recaptcha.CloseDriver();
            if (_testThread != null)
                _testThread.Abort();
        }
    }
}