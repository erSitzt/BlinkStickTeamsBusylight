using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlinkStickTeamsBusylight
{
    public class TeamsLogFile
    {
        private const string statusIndicator = "UserPresenceAction:";
        public readonly string file;
        private string status;

        public TeamsLogFile(string file, TeamsLogFile previousLogFile = null)
        {
            this.file = file;
            status = null;
        }

        public string AnalyzeStatus(string line)
        {
            string statusResult = null;
            var resultStatus = Regex.Match(line, $@"^.*{{.*\bavailability\b:\s*([^\s,}}]*)");
            if (resultStatus.Success)
            {

                statusResult = resultStatus.Groups[1].Value;

            }
            return statusResult;
        }

        public string GetStatus()
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var logFile = new StreamReader(fs))
                {
                    string line;
                    var indicatorLines = new List<string>();
                    while ((line = logFile.ReadLine()) != null)
                    {

                        if (line.Contains(statusIndicator))
                        {
                            indicatorLines.Add(line);
                        }
                    }

                    for (int i = indicatorLines.Count - 1; i >= 0; i--)
                    {
                        status = AnalyzeStatus(indicatorLines[i]);
                        if (status != null)
                        {
                            break;
                        }
                    }
                }
            }
            return status;
        }
    }

    public class NewTeamsStatus
    {
        private string status;
        private TeamsLogFile teamsLogFile;

        public NewTeamsStatus()
        {
            status = null;
            teamsLogFile = null;
        }

        public void GetLogFiles()
        {
            var appdataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localPackagesFolder = Path.Combine(appdataLocal, "Packages");
            var teamsPackagesList = Directory.GetDirectories(localPackagesFolder, "MSTeams_*");
            string teamsLogFile = null;

            foreach (var teamsPackageFolder in teamsPackagesList)
            {
                var teamsLogFileGlob = Path.Combine(teamsPackageFolder, "LocalCache", "Microsoft", "MSTeams", "Logs");
                //var teamsPackagesLogFiles = Directory.GetFiles(teamsLogFileGlob, "MSTeams_*.log");
                var directory = new DirectoryInfo(teamsLogFileGlob);
                var teamsPackagesLogFile = directory.GetFiles("MSTeams_*.log")
                 .OrderByDescending(f => f.LastWriteTime)
                 .First();
                teamsLogFile = teamsPackagesLogFile.FullName;
            }
            this.teamsLogFile = new TeamsLogFile(teamsLogFile);
        }

        public void ProcessLogFiles()
        {
            if (teamsLogFile != null)
            {
                teamsLogFile = new TeamsLogFile(teamsLogFile.file);
            }

            string statusResult = null;
            if (teamsLogFile != null)
            {
                statusResult = teamsLogFile.GetStatus();
            }


            if (statusResult != null)
            {
                status = statusResult;
            }
        }

        public string GetStatus()
        {
            GetLogFiles();
            ProcessLogFiles();
            return status;
        }
    }
}


