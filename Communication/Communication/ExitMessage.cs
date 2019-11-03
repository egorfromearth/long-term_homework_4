using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class ExitMessage : Message
    {
        public ExitMessage(int idRecepient, int idSender) : base(idRecepient, idSender)
        {
            _typeMessage = "ExitMessage";
        }
    }
}
