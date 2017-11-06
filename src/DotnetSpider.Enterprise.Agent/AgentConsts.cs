using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DotnetSpider.Enterprise.Agent
{
    public class AgentConsts
    {
        public static bool IsExited;

        public static string BaseDataDirectory;
        public static string RunningLockPath;
        public static string AgentIdPath;
        public static string ProjectsDirectory;

        public static bool IsRunningOnWindows { get; }

        public static string AgentId
        {
            get { return _agentId; }
            set
            {
                if (_agentId != value)
                {
                    _agentId = value;
                    StoreToConfig();
                }
            }
        }

        public static bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    StoreToConfig();
                }
            }
        }

        private static bool _isEnabled = true;
        private static string _agentId = string.Empty;

        public static void StoreToConfig()
        {
            File.WriteAllText(AgentIdPath, $"{AgentId}{Environment.NewLine}{IsEnabled}");
        }

        public static void LoadConfig()
        {
            if (File.Exists(AgentIdPath))
            {
                var lines = File.ReadAllLines(AgentIdPath);
                _agentId = lines.FirstOrDefault();
                var enabled = false;
                if (lines.Length > 1)
                {
                    bool.TryParse(lines[1], out enabled);
                }
            }
            if (string.IsNullOrEmpty(_agentId))
            {
                _agentId = Guid.NewGuid().ToString("N");
                _isEnabled = true;
                StoreToConfig();
            }
        }

        static AgentConsts()
        {
            IsRunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (IsRunningOnWindows)
            {
                BaseDataDirectory = "c:\\DotnetSpider\\";
            }
            else
            {
                BaseDataDirectory = "/opt/DotnetSpider/";
            }

            if (!Directory.Exists(BaseDataDirectory))
            {
                Directory.CreateDirectory(BaseDataDirectory);
            }

            RunningLockPath = Path.Combine(BaseDataDirectory, "running");
            AgentIdPath = Path.Combine(BaseDataDirectory, "nodeId");
            ProjectsDirectory = Path.Combine(BaseDataDirectory, "projects-agent");

            if (!Directory.Exists(ProjectsDirectory))
            {
                Directory.CreateDirectory(ProjectsDirectory);
            }
        }
    }
}