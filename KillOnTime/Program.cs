using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace KillOnTime
{
    class Program
    {
        private struct ERR
        {
            public const int SUCCESS = 0;
            public const int NOTSTARTED = 0xFD;
            public const int NOTFOUND = 0xFE;
            public const int TIMEOUT = 0xFF;
        }
        static int Main(string[] args)
        {
            if (args.Length < 2 || args.Contains("/?"))
            {
                ShowHelp();
                return ERR.SUCCESS;
            }

            var FileName = FindPathFile(args[1]);
            var Runtime = 0;
            if (!int.TryParse(args[0], out Runtime) || Runtime < 1)
            {
                Console.Error.WriteLine("Invalid time specified");
                ShowHelp();
                return ERR.SUCCESS;
            }
            var Args = args.Length == 2 ? "" : string.Join(" ", args.Skip(2).Select(m => "\"" + m + "\"").ToArray());
            if (!string.IsNullOrEmpty(FileName) && File.Exists(FileName))
            {
                using (var P = new Process())
                {
                    P.StartInfo.UseShellExecute = false;
                    P.StartInfo.FileName = FileName;
                    P.StartInfo.Arguments = Args;

                    try
                    {
                        P.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Unable to launch Process.\nMessage: {1}", ex.Message);
                        return ERR.NOTSTARTED;
                    }
                    if (!P.WaitForExit(Runtime * 1000))
                    {
                        P.Kill();
                        Console.Error.WriteLine("Timeout reached");
                        return ERR.TIMEOUT;
                    }
                }
            }
            else
            {
                Console.Error.WriteLine("Process executable not found");
                return ERR.NOTFOUND;
            }
            return ERR.SUCCESS;
        }

        /// <summary>
        /// Finds the full name of a file by looking at the path variable
        /// </summary>
        /// <param name="Name">File name</param>
        /// <returns>Full name, null if not found</returns>
        private static string FindPathFile(string Name)
        {
            var Segments = (";" + Environment.GetEnvironmentVariable("PATH")).Split(';');
            foreach (var seg in Segments)
            {
                var P = Path.Combine(seg, Name);
                if (File.Exists(P))
                {
                    return P;
                }
            }
            return null;
        }

        /// <summary>
        /// Shows Help
        /// </summary>
        private static void ShowHelp()
        {
            Console.Error.WriteLine(@"KillOnTime.exe <time> <exe> [args]
Runs a process for a maximum given amount of time.
This will kill the process if it does not exits on time.
Any children of the killed process will be left running.

time    - required, maximum time in seconds to wait
exe     - required, exe file to start
args    - optional, arguments to the started process

If the process terminates before the timeout, the process error code is
returned. Otherwise one of these results:
Time expired:                 {0}
Process executable not found: {1}
Problems starting process:    {2}", ERR.TIMEOUT, ERR.NOTFOUND, ERR.NOTSTARTED);
        }
    }
}
