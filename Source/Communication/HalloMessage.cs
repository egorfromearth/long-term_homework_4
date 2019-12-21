using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class HalloMessage : Message
    {

        public SystemInformation Info { get; private set; }

        public HalloMessage(int idRecepient, int idSender, SystemInformation info) : base(idRecepient, idSender)
        {
            TypeMessage = (int)EnumTypeMessage.HelloMessage;
            Info = info;
        }
    }
}
