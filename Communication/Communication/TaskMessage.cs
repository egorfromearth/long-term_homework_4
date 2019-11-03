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

        public Task Task { get => _task; }

        public TaskMessage(int idRecepient, int idSender, Task task) : base(idRecepient, idSender)
        {
            _typeMessage = "TaskMessage";
            _task = task;
        }
    }
}
