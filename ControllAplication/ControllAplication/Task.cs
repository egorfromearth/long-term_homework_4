using System;
using System.Collections.Generic;
using System.Text;

namespace ControllAplication
{
    class Task
    {
        bool _complete;
        string _hash;
        int _idTask;
        bool _processing;
        string readyPassword;

        public bool Complete { get => _complete; set => _complete = value; }
        public string Hash { get => _hash; set => _hash = value; }
        public int IdTask { get => _idTask; set => _idTask = value; }
        public bool Processing { get => _processing; set => _processing = value; }
        public string ReadyPassword { get => readyPassword; set => readyPassword = value; }
    }
}
