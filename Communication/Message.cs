using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class Message //сообщение содержащее id отправимтеля id получателя и тип
    {
        public int IdRecepient { get; protected set; }
        public int IdSender { get; protected set; }
        public int TypeMessage { get; protected set; }

        public Message(int idRecepient, int idSender)
        {
            IdRecepient = idRecepient;
            IdSender = idSender;
            TypeMessage = (int)EnumTypeMessage.Message;
        }
    }
}
