using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwiBot.Model.Repository
{
    [Serializable]
    public class Logs
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
    }
}
