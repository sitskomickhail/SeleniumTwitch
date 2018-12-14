using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwiBot.Model.Repository
{
    [Serializable]
    public class UserRepository
    {
        private IList<Logs> _users;

        public UserRepository()
        {
            _users = new List<Logs>();
        }

        public IList<Logs> GetUser
        {
            get
            {
                return Deserialize();
            }
            protected set
            {
                _users = value;
            }
        }

        public void AddUsers(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] allUsers = File.ReadAllLines(filePath);
                for (int i = 0; i < allUsers.Count(); i++)
                {
                    string[] lpmArray = allUsers[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    _users.Add(new Logs
                    {
                        Login = lpmArray[0],
                        Password = lpmArray[1],
                        Mail = lpmArray[2]
                    });
                }
                Serializer.Serialize<Logs>(_users, "usersBots.txt");
            }
        }


        private IList<Logs> Deserialize()
        {
            if (File.Exists("usersBots.txt"))
            {
                return Serializer.Deserialize<Logs>("usersBots.txt");
            }
            return null;
        }
    }
}
