using System;
using System.Diagnostics;

namespace DotnetSpider.Enterprise.Agent
{
    public class ProcessUtil
    {
        public static void Execute(long taskId, string commandId, string app, string arguments, string workingDirectory)
        {
            Process process = new Process
            {
                StartInfo =
                    {
                        FileName = app,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        WorkingDirectory = workingDirectory,
                        Arguments = arguments,
                        RedirectStandardOutput=true,
                        RedirectStandardError=true
                    }
            };
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += (e, a) =>
            {
                if (a.Data != null)
                {
                    var msg = $"ERROR: {a.Data}" + "\n";
                    Console.WriteLine(msg);

                    if (app != "dotnet")
                    {
                        //ExecuteLog.Insert(new ExecuteLog
                        //{
                        //    CommandId = commandId,
                        //    CreationTime = DateTime.Now,
                        //    LogType = "ERROR",
                        //    Message = a.Data,
                        //    NodeId = AgentConsts.AgentId,
                        //    NodeIp = NodeStatus.GetIpAddress(),
                        //    TaskId = taskId.ToString()
                        //});
                    }
                }
            };
            process.OutputDataReceived += (e, a) =>
            {
                if (a.Data != null)
                {
                    var msg = $"INFO: {a.Data}" + "\n";
                    Console.WriteLine(msg);

                    if (app != "dotnet")
                    {
                        //ExecuteLog.Insert(new ExecuteLog
                        //{
                        //    CommandId = commandId,
                        //    CreationTime = DateTime.Now,
                        //    LogType = "INFO",
                        //    Message = a.Data,
                        //    NodeId = AgentConsts.AgentId,
                        //    NodeIp = NodeStatus.GetIpAddress(),
                        //    TaskId = taskId.ToString()
                        //});
                    }
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

    }
}