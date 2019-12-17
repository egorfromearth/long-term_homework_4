using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class TaskStopProcessingMessage:Message
    {
        public int IdTask { get; set; }

        public TaskStopProcessingMessage(int idRecepient, int idSender, int idTask) : base(idRecepient, idSender)
        {
            TypeMessage = (int)EnumTypeMessage.TaskStopProcessingMessage;
            IdTask = idTask;
        }
    }
}
