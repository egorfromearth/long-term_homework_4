using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class PingMessage:Message
    {
        public PingMessage(int idRecepient, int idSender) : base(idRecepient, idSender)
        {
            TypeMessage = (int)EnumTypeMessage.PingMessage;
        }
    }
}
