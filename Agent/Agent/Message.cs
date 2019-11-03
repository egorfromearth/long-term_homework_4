﻿using System;
using System.Collections.Generic;


namespace Agent
{
    [Serializable]
    class Message //сообщение содержащее id отправимтеля id получателя и тип
    {
        protected int _idRecepient;
        protected int _idSender;
        protected string _typeMessage;

        public int IdRecepient { get => _idRecepient; }
        public int IdSender { get => _idSender; }
        public string TypeMessage { get => _typeMessage; }

        public Message(int idRecepient, int idSender)
        {
            _idRecepient = idRecepient;
            _idSender = idSender;
            _typeMessage = "Message";
        }
    }

    [Serializable]
    class TaskMessage : Message
    {
        private Dictionary<int, string> _task;

        public Dictionary<int, string> Task { get => _task; }

        public TaskMessage(int idRecepient, int idSender, Dictionary<int, string> task) : base(idRecepient, idSender)
        {
            _typeMessage = "TaskMessage";
            _task = task;
        }
    }
}

