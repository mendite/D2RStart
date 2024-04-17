using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace D2RStart
{
    public static class D2RProcessManager
    {
        private class D2RExeInfo
        {
            public string Pid { get; set; }
            public string EventId { get; set; }

            public override string ToString()
            {
                return $"{nameof(Pid)}:{Pid}, {nameof(EventId)}:{EventId}";
            }
        }

        private static D2RExeInfo GetD2RExeInfo(bool firstRun = true)
        {
            Process processListFilehandles = new Process();
            processListFilehandles.StartInfo.CreateNoWindow = true;
            processListFilehandles.StartInfo.FileName = "handle64.exe";
            processListFilehandles.StartInfo.Arguments = "-vt -a -p D2R.exe -nobanner";
            processListFilehandles.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processListFilehandles.StartInfo.UseShellExecute = false;
            processListFilehandles.StartInfo.RedirectStandardError = true;
            processListFilehandles.StartInfo.RedirectStandardOutput = true;

            var stdOutput = new StringBuilder();

            processListFilehandles.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data);

            string stdError = null;

            processListFilehandles.Start();
            processListFilehandles.BeginOutputReadLine();
            stdError = processListFilehandles.StandardError.ReadToEnd();
            processListFilehandles.WaitForExit();

            Regex regexNoHandlesFound = new Regex("No matching handles found\\..*");
            if (regexNoHandlesFound.IsMatch(stdOutput.ToString()))
                return null;

            if (processListFilehandles.ExitCode != 0)
            {
                if (string.IsNullOrEmpty(stdError) && firstRun)
                {
                    Process processAgreeLicense = new Process();
                    processAgreeLicense.StartInfo.CreateNoWindow = true;
                    processAgreeLicense.StartInfo.FileName = "handle64.exe";
                    processAgreeLicense.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processAgreeLicense.StartInfo.UseShellExecute = false;
                    processAgreeLicense.Start();
                    processAgreeLicense.WaitForExit();

                    if (processAgreeLicense.ExitCode == 0)
                    {
                        return GetD2RExeInfo(false);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Declined license agreement for {processAgreeLicense.StartInfo.FileName}?");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(stdError))
                        throw new InvalidOperationException($"Unexpected error occured at executing {processListFilehandles.StartInfo.FileName} ({processListFilehandles.ExitCode}).");

                    throw new InvalidOperationException(stdError);
                }               
            }
            
            //D2R.exe	7528	QUIET\mendi	0x000007F0	Event		\Sessions\1\BaseNamedObjects\DiabloII Check For Other Instances 
            Regex regexPid = new Regex(@"(?<=D2R.exe\t)(?<PID>\d+)\t.*\t(?<EVENTID>(0x\w{8}))\tEvent\t.*DiabloII Check For Other Instances");
            Match match = regexPid.Match(stdOutput.ToString());

            if (!match.Success)
                return null;

            string pid = match.Groups.Cast<Group>().Where(X => X.Name == "PID").FirstOrDefault()?.Value;
            string eventId = match.Groups.Cast<Group>().Where(X => X.Name == "EVENTID").FirstOrDefault()?.Value;

            Debug.WriteLine($"Found handle PID:{pid}, EVENTID:{eventId}");

            return new D2RExeInfo()
            {
                Pid = pid,
                EventId = eventId,
            };
        }

        private static string ExecuteHandle64(string args)
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "handle64.exe";
            process.StartInfo.Arguments = args;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            var stdOutput = new StringBuilder();

            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data);

            string stdError = null;

            process.Start();
            process.BeginOutputReadLine();
            stdError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Regex regexNoHandlesFound = new Regex("No matching handles found\\..*");
            if (regexNoHandlesFound.IsMatch(stdOutput.ToString()))
                return null;

            if (process.ExitCode != 0)
                throw new Exception(stdError);

            return stdOutput.ToString();
        }

        public static void RemoveHandle()
        {
            D2RExeInfo d2rExeInfo = GetD2RExeInfo();

            if (d2rExeInfo == null)
                return;

            //handle64 -p %pid% -c %hex% -y -nobanner
            string stdOutput = ExecuteHandle64($"handle64 -p {d2rExeInfo.Pid} -c {d2rExeInfo.EventId} -y -nobanner");

            Debug.WriteLine($"Removed handle PID:{d2rExeInfo.Pid}, EVENTID:{d2rExeInfo.EventId}");
        }

        public static bool D2ROfPathIsRunning(string d2rPath)
        {
            //Process process = Process.GetProcesses().Where(p => p.ProcessName.ToUpper() == "D2R" && p.MainModule?.FileName?.ToLower() == d2rPath?.ToLower()).FirstOrDefault();
            string exeFilename = Path.Combine(d2rPath, "d2r.exe");
            Process process = Process.GetProcesses().Where(p => p.ProcessName.ToUpper() == "D2R" && p.MainModule?.FileName?.ToLower() == exeFilename?.ToLower()).FirstOrDefault();
            return process != null;
        }

        public static void StartD2RLauncherByPath(string d2rPath)
        {
            if (!Directory.Exists(d2rPath))
                throw new DirectoryNotFoundException($"D2R path '{d2rPath}' was not found.");

            string launcherFile = Path.Combine(d2rPath, "Diablo II Resurrected Launcher.exe");
            Process.Start(launcherFile);
        }
    }
}
