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
        int _countCore;
        int _passwordsPerSecond;

        public int CountCore { get => _countCore; set => _countCore = value; }
        public int PasswordsPerSecond { get => _passwordsPerSecond; set => _passwordsPerSecond = value; }

    }
}
