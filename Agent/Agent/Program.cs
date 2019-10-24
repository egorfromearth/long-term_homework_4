using System;
using System.Collections.Generic;
using System.Linq;
namespace Agent
{
       
    class Agent
    {
        private int _idAgent;
   
        private Dictionary<int, string> tasks; //Id/hash
        public int IdAgent { get => _idAgent; set => _idAgent = value; }

        static void Main(string[] args)
        {

        }

    }

    struct systemInformation
    {
          int _core;
          double _passwordsPerSecond;
    }

    class PasswordGuessing
    {
        private string _alphabet;
        private string _hash;
        private string _password;

        public PasswordGuessing(string alphabet, string hash)
        {
            _alphabet = alphabet;
            _hash = hash;
            _password = "";
        }
        public bool BruteForce(string beginRange, string endRange, string password)
        {
            return true;
        }

        public string getPassword()
        {
            return _password;
        }
    }

    class Message
    {
        private int _idRecepient;
        private int _idSender;
        private string _message;
        public Message(int idRecepient, int idSender, string message)
        {
            _idRecepient = idRecepient;
            _idSender = idSender;
            _message = message;
        }

        public virtual string[] getData()
        {
            return null;
        }
    }

    class Communication
    {
       bool SendMessage(Message message, string path) //путь к очереди
        {
            return true;
        }

        Message CheckMessage(string path)
        {
            return null;
        }
        
    }

}



