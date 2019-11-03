using System;
using System.Collections.Generic;
using System.Text;

namespace ControllAplication
{
    class SystemInformation
    {
        int _countCore;
        double _passwordsPerSecond;

        int CountCore { get => _countCore; }
        double IdTaskt { get => _passwordsPerSecond; }

        SystemInformation(int countCore, double passwordsPerSecond)
        {
            _countCore = countCore;
            _passwordsPerSecond = passwordsPerSecond;
        }
    }
}
