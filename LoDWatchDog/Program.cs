using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LoDWatchDog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Watching for server crashes...");

            while (true)
            {
                Process[] processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    if (!String.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        // Check for an engine error popup
                        if (process.MainWindowTitle == "Engine Error")
                        {
                            // Kill mr process
                            process.Kill();
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
