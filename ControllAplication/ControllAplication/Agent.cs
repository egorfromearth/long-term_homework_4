using System;
using System.Collections.Generic;
using System.Text;

namespace ControllAplication
{
    class Agent
    {
        int _idAgent;
        int _idTask;
        SystemInformation _info;

        int IdAgent { get => _idAgent; }
        int IdTaskt { get => _idTask; }
        SystemInformation Info { get => _info; }

        public Agent(int idAgent, int idTask, SystemInformation info)
        {
            _idAgent = idAgent;
            _idTask = idTask;
            _info = info;
        }
    }
}
