using System;
using System.Collections.Generic;
using System.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Communication;
using ControlAplication;

namespace ControllAplication
{
    

    class ControlApplication
    {
        private static char[] charactersToTest =
{
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','x','y','z',
            'А','Б','В','Г','Д','Е','Ё','Ж','З','И','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я',
            'а','б','в','г','д','е','ё','ж','з','и','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я',
            '1','2','3','4','5','6','7','8','9','0'
        };

        /// <summary>
        /// Сдвигает последовательность букв на заданную длину.
        /// </summary>
        /// <param name="start">Стартовая строка, которую мы будем двигать.</param>
        /// <param name="len">Число раз, на которое будем двигать.</param>
        public static string lineShift(string start, ulong len)
        {
            //"BCD";
            ulong decStart = 0;

            int[] indexes = new int[start.Length];
            for (int index = 0; index < start.Length; ++index)
                indexes[index] = Array.IndexOf(charactersToTest, start[index]);

            for (int index = 0; index < start.Length; ++index)
                decStart += (ulong)(Math.Pow(charactersToTest.Length, index) * (indexes[index] + 1));

            decStart += len;
            ulong temp = decStart;
            int count = 0;
            while (temp != 0)
            {
                temp /= (ulong)charactersToTest.Length;
                ++count;
            }
            string res = "";
            StringBuilder result = new StringBuilder(res, 6);
            for (int index = 0; index < count; ++index)
            {
                int num = (int)(decStart / Math.Pow(charactersToTest.Length, count - index - 1));
                result.Append(charactersToTest[num - 1]);
                decStart %= (ulong)Math.Pow(charactersToTest.Length, count - index - 1);
            }

            string r = result.ToString();
            return r;


        }

        public static string reverseStr(string str)
        {
            char[] inputarray = str.ToCharArray();
            Array.Reverse(inputarray);
            string output = new string(inputarray);
            return output;
        }

        public static void Distribution(List<Agent> agentsList, Task task)
        {
            ulong[] speedArr = new ulong[agentsList.Count];

            for (int i = 0; i < agentsList.Count; ++i)
            {
                speedArr[i] = (ulong)agentsList[i].Info.PasswordsPerSecond;
            }

            string[,] myArr = new string[speedArr.Length, 2];
            ulong[] count_pass = new ulong[speedArr.Length];
            ulong sum = 0;

            for (int index = 0; index < speedArr.Length; ++index)
                sum += speedArr[index];

            const ulong countCombination = 3664769671500; //3664769671500; 
            ulong time = countCombination / sum;
            string start = "A";
            for (int index = 0; index < count_pass.Length - 1; ++index)
            {

                myArr[index, 0] = start;
                myArr[index, 1] = lineShift(reverseStr(myArr[index, 0]), (time * speedArr[index]));
                start = lineShift(reverseStr(myArr[index, 1]), 1);
            }

            if (speedArr.Length != 1)
            {
                myArr[speedArr.Length - 1, 0] = lineShift(reverseStr(myArr[speedArr.Length - 2, 1]), 1);
                myArr[speedArr.Length - 1, 1] = "000000";
            }
            else {
                myArr[0, 0] = "A";
                myArr[0, 1] = "000000";
            }

            for (int i = 0; i < speedArr.Length; ++i)
            {
                TaskMessage message = new TaskMessage(agentsList[i].IdAgent, 0, task, myArr[i, 0], myArr[i, 1]);
                agentsList[i].QueueReceive.Send(message);
            }
        }



        private static int _countHash = 0;
        private static int _nextIdTask = 1;

        public static void recervingHalloMessage()
        {
            while (true)
            {
                ProcessingMessage.ChekMessage(ProcessingMessage.mainQueue);
            }
        }

        


        static void Main(string[] args)
        {
            if (!MessageQueue.Exists(@".\private$\MainQueue"))
            {
                MessageQueue.Create(@".\private$\MainQueue");
            }

            ProcessingMessage.mainQueue = new MessageQueue(@".\private$\MainQueue");
            ProcessingMessage.mainQueue.Formatter = new BinaryMessageFormatter();

            Thread threadAgent = new Thread(recervingHalloMessage);
            threadAgent.Start();
            string read = "";
            do
            {
                Console.Write("1. Добавить задание\n");
                Console.Write("2. Посмотреть список заданий\n");
                Console.Write("3. Выйти\n");

                read = Console.ReadLine();
                if (read == "1")
                {
                    Console.Write("Введите колличество сверток\n");
                    _countHash = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Введите все свертки по одной\n");
                    for (int index = 0; index < _countHash; ++index)
                    {
                        Task task = new Task(Console.ReadLine(), _nextIdTask);
                        ++_nextIdTask;

                        ProcessingMessage.tasksList.Add(task);
                    }
                    Thread.Sleep(5000);

                    foreach (var item in ProcessingMessage.tasksList) {
                        Distribution(ProcessingMessage.agentsList, item);
                    }

                    /*
                    место для раскидывания заданий агентам
                    */
                }
                else if(read == "2") {
                    foreach (var item in ProcessingMessage.tasksList) {
                        Console.Write(" Hash: " + item.Hash + "\n Complete: " + item.Complete + "\n ReadyPassword: " + item.ReadyPassword + '\n');
                        Console.Write('\n');
                    }
                }

            } while (read != "3");


           /* 


            while (ProcessingMessage.agentsList.Capacity == 0);

            Thread.Sleep(2000);

            var md5 = MD5.Create();
            var tempHash = md5.ComputeHash(Encoding.UTF8.GetBytes("assdw"));
            var hash = Convert.ToBase64String(tempHash);

            Task newTask = new Task();
            newTask.Complete = false;
            newTask.Hash = hash;
            newTask.IdTask = 1;
            newTask.Processing = false;
            newTask.ReadyPassword = "";

            TaskMessage taskMessage = new TaskMessage(0, ProcessingMessage.agentsList[0].IdAgent, newTask, "aa", "bbb");
            ProcessingMessage.agentsList[0].QueueReceive.Send(taskMessage);
                   
            Thread.Sleep(5000);*/

            foreach (Agent item in ProcessingMessage.agentsList)
            {

               
                ExitMessage exit = new ExitMessage(item.IdAgent, 0);
                item.QueueReceive.Send(exit);
                ProcessingMessage.dictionaryThread[item].Abort();
                ProcessingMessage.dictionaryThread[item].Join();
            }

            foreach (string item in ProcessingMessage.quereList)
            {
                MessageQueue.Delete(item);
            }

            threadAgent.Abort();

            Environment.Exit(0);
        }
    }
}
