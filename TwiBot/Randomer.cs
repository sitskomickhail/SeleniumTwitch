using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwiBot
{
    public static class Randomer
    {
        private static Random _rand;

        static Randomer()
        {
            _rand = new Random();
        }

        public static int Next(int num1, int num2)
        {
            return _rand.Next(num1, num2);
        }
    }
}
