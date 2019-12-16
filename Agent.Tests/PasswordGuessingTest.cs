using System;
using System.Messaging;
using Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Agent;


namespace Agent.Tests
{
    [TestClass]
    public class PasswordGuessingTest
    {
        [TestMethod]
        public void GetHashTestNormal()
        {
            string[] strArr = { "A", "V6", "7yы", "уц83", "AЬSЫФ", "ннSрФ7" };
            string[] hashArr = { "f8VicOenD6gaWTW3Lqy+KQ==", "25Pxw1uEQzTismaVFhi0/g==", "X/9hyYHjo93MWDN7La1ckg==", "d+GgbOH8uYNdNFVkep3Shw==", "PoaU3beWAR7RkeBhGrW6fw==", "nWJ4Bi1izyUspFKVoRZd/w==" };
            for (int index = 0; index < hashArr.Length; ++index)
              Assert.AreEqual(hashArr[index], PasswordGuessing.GetHash(strArr[index]));
        }


   
        


        [TestMethod]
        public void BruteForceTestNormal()
        {
            int len = 4;
            string start = "ABCD";
            string stop = "EEEE";
            string hash = "XiUYNkNaF+bVhDyNvfaehw==";
            bool flag = false;
            flag = PasswordGuessing.BruteForce(len, start, stop, hash);
            Assert.IsTrue(flag);
        }


        [TestMethod]
        public void BruteForceTestConvertError()
        {
            int len = 3;
            string start = "ABC";
            string stop = "EEE";
            string hash = "XiYNkNaF+bhDyNvfaehw=";
            bool flag = false;
            flag = PasswordGuessing.BruteForce(len, start, stop, hash);
            Assert.IsFalse(flag);
            
        }

        [TestMethod]
        public void BruteForceTestOutOfRange()
        {
            int len = 3;
            string start = "ABC";
            string stop = "EEE";
            string hash = "bWiQAOHWQr3k2UFmaELw8g==";
            bool flag = false;
            flag = PasswordGuessing.BruteForce(len, start, stop, hash);
            Assert.IsFalse(flag);
        }

        [TestMethod]
        public void lineShift()
        {
            ulong len = 10;
            string start = "AAA";
            string stop = "AAK";
            Assert.AreEqual(stop, PasswordGuessing.lineShift(start, len));
        }

        [TestMethod]
        public void lineShiftLineRange()
        {
            ulong len = 1;
            string start = "000000";
            string stop = "AAAAAAA";
            Assert.AreEqual(stop, PasswordGuessing.lineShift(start, len));
        }


        
    }
}
