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
        private SystemInformation _info;

        public SystemInformation info { get => _info; }

        public HalloMessage(int idRecepient, int idSender, SystemInformation info) : base(idRecepient, idSender)
        {
            _typeMessage = "HalloMessage";
            _info = info;
        }
    }
}
