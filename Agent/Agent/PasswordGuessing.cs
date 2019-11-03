using System;
using System.Collections.Generic;
using System.Text;

namespace Agent
{
    class PasswordGuessing
    {
        private string _alphabet;
        private string _hash;
        private string _password;

        public PasswordGuessing(string alphabet, string hash, string beginRange, string endRange)
        {
            _alphabet = alphabet;
            _hash = hash;
            _password = "";
        }
        public bool BruteForce()
        {
            return true;
        }

        public string getPassword()
        {
            return _password;
        }
    }
}
