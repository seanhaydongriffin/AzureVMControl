using System;
using System.IO;
using System.Reflection;

namespace SharedProject
{
    class Log
    {
        static public string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + Assembly.GetCallingAssembly().GetName().Name + ".log";


        static public void Initialise(string log_path)
        {
            if (log_path != null)

                path = log_path.ReplaceIgnoreCase("/LOG:", "");

            if (System.IO.File.Exists(path))

                System.IO.File.Delete(path);
        }

        static public void WriteLine(string message)
        {
            Console.WriteLine(message);

            using (StreamWriter w = System.IO.File.AppendText(path))
            {
                w.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} : {message}");
            }
        }


    }
}
