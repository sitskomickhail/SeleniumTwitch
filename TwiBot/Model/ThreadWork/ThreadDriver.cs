using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TwiBot.Model.Repository;

namespace TwiBot.Model.ThreadWork
{
    public class ThreadDriver
    {
        private List<Thread> _threads;
        private List<RecaptchaV2_Solving> _captchas;
        private UserRepository _users;
        private int _usersCount;
        private Thread _testThread;
        private TestRecaptcha _recaptcha;

        public ThreadDriver()
        {
            _threads = new List<Thread>();
            _captchas = new List<RecaptchaV2_Solving>();
            _users = new UserRepository();
            _recaptcha = new TestRecaptcha();
            //for (int i = 0; i < _users.GetUser.Count; i++) { _captchas.Add(new RecaptchaV2_Solving()); }
        }

        public void StartDriver_Work(string url, int? count = null)
        {
            _usersCount = count ?? _users.GetUser.Count;
            int i = 0;
            foreach (var user in _users.GetUser)
            {
                //TODO: normal count
                _threads.Add(new Thread(() => _captchas[i].GoTo_Twitch(url, user.Login, user.Password))); //TODO: Position for download
                _threads[i].Start();
                if (i == 1)                
                    break;
                i++;
            }
        }

        public void TestStartDriver_Work(string url)
        {
            _testThread = new Thread(() => _recaptcha.GoTo_Twitch(url));
            _testThread.Start();
        }

        public int ReturnThreadsCount() { return _threads.Count; }

        public void Kill_Threads()
        {
            foreach (RecaptchaV2_Solving capDriver in _captchas)
            {
                capDriver.CloseDriver();
            }
            foreach (Thread thread in _threads)
            {
                thread.Abort();
            }
        }
    }
}