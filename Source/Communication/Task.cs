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
        public Task(string hash, int idTask) {
            Complete = false;
            Hash = hash;
            IdTask = idTask;
            Processing = false;
            ReadyPassword = null;
        }

        public bool Complete { get; set; }
        public string Hash { get; private set;}
        public int IdTask { get; private set; }
        public bool Processing { get; set; }
        public string ReadyPassword { get; set; }
    }
}
