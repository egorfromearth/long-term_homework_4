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

        public string Start { get; private set; }
        public string Stop { get; private set; }

        public Task Task { get; private set; }

        public TaskMessage(int idRecepient, int idSender, Task task, string start, string stop) : base(idRecepient, idSender)
        {
            TypeMessage = (int)EnumTypeMessage.TaskMessage;
            Task = task;
            Start = start;
            Stop = stop;
        }
    }
}
