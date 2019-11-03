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
        protected int _idRecepient;
        protected int _idSender;
        protected string _typeMessage;

        public int IdRecepient { get => _idRecepient; }
        public int IdSender { get => _idSender; }
        public string TypeMessage { get => _typeMessage; }

        public Message(int idRecepient, int idSender)
        {
            _idRecepient = idRecepient;
            _idSender = idSender;
            _typeMessage = "Message";
        }
    }
}
