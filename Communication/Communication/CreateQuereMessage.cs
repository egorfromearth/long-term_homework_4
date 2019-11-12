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
        private string _pathSend;
        private string _pathReceive;

        public string PathSend { get => _pathSend; }
        public string PathReceive { get => _pathReceive; }

        public CreateQuereMessage(int idRecepient, int idSender, string pathSend, string pathReceive) : base(idRecepient, idSender)
        {
            _typeMessage = "CreateQuereMessage";
            _pathSend = pathSend;
            _pathReceive = pathReceive;
        }
    }
}
