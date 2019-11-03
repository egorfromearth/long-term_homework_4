using System;
using System.Collections.Generic;
using System.Text;
using Communication;

namespace ControllAplication
{
    public class Agent
    {
        int _idAgent;
        int _idTask;
        string _quere;
        SystemInformation _info;

        public int IdAgent { get => _idAgent; }
        public int IdTaskt { get => _idTask; }
        public string Quere { get => _quere; }
        SystemInformation Info { get => _info; }

        public Agent(int idAgent, int idTask, SystemInformation info, string quere)
        {
            _idAgent = idAgent;
            _idTask = idTask;
            _quere = quere;
            _info = info;
        }
    }
}
