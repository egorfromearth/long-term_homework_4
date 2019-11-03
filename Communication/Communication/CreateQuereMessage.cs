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
        private string _path;

        public string Path { get => _path; }

        public CreateQuereMessage(int idRecepient, int idSender, string path) : base(idRecepient, idSender)
        {
            _typeMessage = "CreateQuereMessage";
            _path = path;
        }
    }
}
