using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class CreateQuereMessage : Message
    {
        public string PathSend { get; private set; }
        public string PathReceive { get; private set; }

        public CreateQuereMessage(int idRecepient, int idSender, string pathSend, string pathReceive) : base(idRecepient, idSender)
        {
            TypeMessage = (int)EnumTypeMessage.CreateQuereMessage;
            PathSend = pathSend;
            PathReceive = pathReceive;
        }
    }
}
