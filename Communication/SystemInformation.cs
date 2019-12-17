using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class SystemInformation
    {

        public int CountCore { get; set; }
        public int PasswordsPerSecond { get; set; }

    }
}
