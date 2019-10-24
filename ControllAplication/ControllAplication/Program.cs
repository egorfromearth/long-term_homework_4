using System;
using System.Collections.Generic;
using System.Linq;
namespace ControllAplication
{

    class ControlApplication
    {
        private List<Task> _tasksList;
        private List<Agent> _tasksAgent;

        public void distributionTasks() { }
        static void Main(string[] args)
        {

        }
    }


    struct Task
    {
        bool _complete;
        string _hash;
        int _idTask;
        bool _processing;
        string readyPassword;
    }


    struct Agent
    {
        int _idAgent;
        int _idTask;
        systemInformation info;
    }
    struct systemInformation
    {
        int _core;
        double _passwordsPerSecond;
    }







    class Message
    {
        private int _idRecepient;
        private int _idSender;
        private string _message;
        public Message(int idRecepient, int idSender, string message)
        {
            _idRecepient = idRecepient;
            _idSender = idSender;
            _message = message;
        }

        public virtual string[] getData()
        {
            return null;
        }
    }

    class Communication
    {
        bool SendMessage(Message message, string path) //путь к очереди
        {
            return true;
        }

        Message CheckMessage(string path)
        {
            return null;
        }

    }

}
