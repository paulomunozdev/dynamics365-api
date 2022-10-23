using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JumpStart.Log
{
    public static class FileLog
    {
       
        public static void WriteTimer(string message, double ElapsedSeconds)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            using (FileStream fs = new FileStream(GetFileName(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs,Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now + " | " + message + ": " + ElapsedSeconds + " Sec    / " +
                    ((float)ElapsedSeconds / (float)60).ToString("N2") +
                    " min");
                    sw.WriteLine();
                }
            }
        }

        public static void Write(string message, params string[] obj)
        {
            Write(String.Format(message, obj));
        }

        public static void Write(string message)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            using (FileStream fs = new FileStream(GetFileName(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now + " | " + message);
                    sw.WriteLine();
                }
            }
        }

        public static void Write(string folderName, string message)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Logs");

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            using (FileStream fs = new FileStream(GetFileName(folderName), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now + " | " + message);
                    sw.WriteLine();
                }
            }
        }

        private static string GetFileName()
        {
            return  @"Logs\log_" + DateTime.Today.Ticks + ".txt";
        }

        private static string GetFileName(string folderName)
        {
            return String.Format(@"Logs\{0}\log_", folderName) + DateTime.Today.Ticks + ".txt";
        }
    }
}