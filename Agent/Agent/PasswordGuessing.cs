using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography; //Для MD5
using System.Threading;

namespace Agent
{


    class PasswordGuessing
    {


        private static char[] charactersToTest =
        {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','x','y','z',
            'А','Б','В','Г','Д','Е','Ё','Ж','З','И','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я',
            'а','б','в','г','д','е','ё','ж','з','и','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я',
            '1','2','3','4','5','6','7','8','9','0'
        };


        private string _pass;

        public string Psss { get => _pass; }

        public PasswordGuessing()
        {
            _pass = "";
        }

        public int SpeedTest()
        {
            var timeStarted = DateTime.Now;
            // Console.WriteLine("Start BruteForce - {0}", timeStarted.ToString());
            Brute("AAA", "000", "000");
            // Console.WriteLine("Password matched. - {0}", DateTime.Now.ToString());

            //Console.WriteLine("Time passed: {0}s", DateTime.Now.Subtract(timeStarted).TotalSeconds);

            double speed_buf = (1906624 / (DateTime.Now.Subtract(timeStarted).TotalSeconds));
            int speed = (int)speed_buf;
            return speed;
        }

        /// <summary>
        /// Возвращает хеш строки
        /// </summary>
        /// <param name="input">строка, хэш которой нужно получить</param>
        /// <returns>Строку, являющуюся хешем строки</returns>
        public static string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }


        /// <summary>
        /// Риверс строки
        /// </summary>
        /// <param name="str">Строка которую нужно риверснуть</param>
        /// <returns>Риверснутая строка</returns>
        public static string reverseStr(string str)
        {
            char[] inputarray = str.ToCharArray();
            Array.Reverse(inputarray);
            string output = new string(inputarray);
            return output;
        }
        /// <summary>
        /// Делает подбор пароля от start До stop размера n
        /// </summary>
        /// <param name="n">Длина подбираемого пароля</param>
        /// <returns>True - если пароль найдет, false - если пароль не нашли. </returns>
        public bool BruteForce(int n, string start, string stop, string hash)
        {
            int imin = 0;
            int imax = charactersToTest.Length - 1;

            //pswd = reverseStr(pswd);
            start = reverseStr(start);
            stop = reverseStr(stop);
            
            //string hash = (GetHash(pswd));

            //Массив для индексов стартовой последовательности
            int[] indexes = new int[n];


            for (int index = 0; index < n; ++index)
                indexes[index] = Array.IndexOf(charactersToTest, start[index]);


            while (indexes[n - 1] != imax + 1)
            {
                string pass_buf = "";
                StringBuilder pass_buff = new StringBuilder(pass_buf, 6);


                for (int index = 0; index < n; ++index)
                    pass_buff.Append(charactersToTest[indexes[index]]); //Аналог присовения для данного метода.




                pass_buf = pass_buff.ToString();

                // Console.WriteLine(pass);

                if (GetHash(reverseStr(pass_buf)) == hash)
                {
                    _pass = reverseStr(pass_buf);
                    return true;
                }

                if (pass_buf == stop)
                    return false;

                ++indexes[0];

                for (int index = 0; index < n - 1; ++index)
                {
                    if (indexes[index] > imax)
                    {
                        indexes[index] = imin;
                        ++indexes[index + 1];
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Функция подобра пароля.
        /// </summary>
        /// <param name="start">Диапазон, с которого начинается поиск</param>
        /// <param name="stop">Конечная граница поиска</param>
        /// <param name="pswd">Свертка</param>
        /// <returns>Найде/не найден пароль</returns>
        public void Brute(string start, string stop, string pswd)
        {
            bool flag = false;
            string[] startArr = { "A", "AA", "AAA", "AAAA", "AAAAA", "AAAAAA" };
            string[] stopArr = { "0", "00", "000", "0000", "00000", "000000" };
            int startLen = start.Length;
            int stopLen = stop.Length;

            if (startLen != stopLen)
            {
                flag = BruteForce(startLen, start, stopArr[startLen - 1], pswd);
                ++startLen;
            }


            while ((startLen != stopLen + 1) && !flag)
            {

                if (startLen == stopLen)
                    flag = BruteForce(startLen, startArr[startLen - 1], stop, pswd);
                else
                    flag = BruteForce(startLen, startArr[startLen - 1], stopArr[startLen - 1], pswd);

                ++startLen;
            }

        }
    }
}

