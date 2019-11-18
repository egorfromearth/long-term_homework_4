using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    
    [Serializable]
    public class TaskMessage : Message
    {
        private Task _task;
        private string _start;
        private string _stop;

        public string Start { get => _start; }
        public string Stop { get => _stop; }

        public Task Task { get => _task; }

        public TaskMessage(int idRecepient, int idSender, Task task, string start, string stop) : base(idRecepient, idSender)
        {
            _typeMessage = "TaskMessage";
            _task = task;
            _start = start;
            _stop = stop;
        }
    }
}
