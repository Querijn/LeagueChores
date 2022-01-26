using System.Diagnostics;
using System.Text;
using System.Management;

namespace LeagueChores
{
    internal static class ProcessExtensions
    {
        public static string GetCommandLine(this Process instance)
        {
            if (instance.MainModule == null)
                return "";

            var cmdLineString = new StringBuilder(instance.MainModule.FileName);

            cmdLineString.Append(" ");
            using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + instance.Id))
            {
                foreach (var obj in searcher.Get())
                {
                    cmdLineString.Append(obj["CommandLine"]);
                    cmdLineString.Append(" ");
                }
            }

            return cmdLineString.ToString();
        }
    }
}
