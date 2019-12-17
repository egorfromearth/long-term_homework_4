using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class Task
    {
        string _hash;
        int _idTask;
        bool _processing;
        string _readyPassword;

        public Task(string hash, int idTask) {
             Complete = false;
             _hash = hash;
             _idTask = idTask;
             _processing = false;
             _readyPassword = null;
        }

        public bool Complete { get; set; }
        public string Hash { get => _hash;}
        public int IdTask { get => _idTask;}
        public bool Processing { get => _processing; set => _processing = value; }
        public string ReadyPassword { get => _readyPassword; set => _readyPassword = value; }
    }
}
