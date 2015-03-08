using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoDWatchDog
{
    class Program
    {
        // The minidump error
        private const string MinidumpError = "Setting breakpad minidump AppID = 580";

        static void log(string str)
        {
            Console.WriteLine(str);
        }

        static void Main(string[] args)
        {
            // Port to use
            int port = 27016;

            // Max slots on the server
            int maxPlayers = 24;

            // Map to load
            string map = "dota";

            string app = "srcds.exe";
            string serverArgs = "-console -game dota +maxplayers " + maxPlayers + " -port " + port + " +dota_force_gamemode 15 +dota_local_addon_enable 1 +map " + map;

            // Build the command to start the server
            var proc = new ProcessStartInfo
            {
                FileName = app,
                Arguments = serverArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            // Loop monitor
            while (true)
            {
                // Start the process
                var process = Process.Start(proc);
                if (process == null) continue;;

                // Error manager
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    // Wait for an error (if process exits, this will be null)
                    var stderrx = process.StandardError.ReadLine();

                    log("HMMM");

                    // Check for minidump error
                    if (stderrx != MinidumpError) log(stderrx);

                    // Ensure the process is dead
                    if (!process.HasExited) process.Kill();
                }, null);

                // Continue to read stdout until the server closes
                while (true)
                {
                    if (process == null) break;

                    // Read a line and check if it;s the end of our input
                    var line = process.StandardOutput.ReadLine();
                    if (line == null) break;

                    // Log it
                    log(line);

                    // Check for entity crash
                    if (line == "Spewing edict counts:") break;
                }

                // Ensure the process is dead
                if (!process.HasExited) process.Kill();
            }
        }
    }
}
