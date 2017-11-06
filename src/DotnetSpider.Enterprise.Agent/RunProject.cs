using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
    public class RunProject : BaseHandler
    {
        public RunArgument Argument { get; set; }

        public class RunArgument
        {
            public int SolutionId { get; set; }
            public long TaskId { get; set; }
            public string Entry { get; set; }
            public string SpiderName { get; set; }
            public string ExecuteArguments { get; set; }
            public string ProjectName { get; set; }
            public string Version { get; set; }
            public string Identity { get; set; }
        }

        public RunProject() : base(new Command())
        {
        }

        public RunProject(Command command) : base(command)
        {
            Argument = JsonConvert.DeserializeObject<RunArgument>(Command.Data);
        }

        public override void Handle()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(Argument.Version))
                    {
                        return;
                    }
                    var key = new ProjectKey
                    {
                        SolutionId = Argument.SolutionId,
                        ProjectName = Argument.ProjectName,
                        Version = Argument.Version
                    };

                    var directory = Path.Combine(AgentConsts.ProjectsDirectory, Argument.ProjectName);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var publishTargetDirectory = Path.Combine(directory, Argument.Version, Argument.SpiderName);

                    // check version exits
                    if (!Directory.Exists(publishTargetDirectory))
                    {
                        var filePath = Path.Combine(directory, $"{Argument.Version}.zip");
                        if (!File.Exists(filePath))
                        {
                            var downloadUrl = Config.Instance.ProjectDownloadUrl;
                            var r = GetResponse(downloadUrl, key);
                            if (r == null)
                            {
                                return;
                            }
                            var bytes = r.Content.ReadAsByteArrayAsync().Result;

                            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                            }
                        }
                        ZipUtil.UnZip(filePath, publishTargetDirectory);
                    }
                    else
                    {
                        //ExecuteLog.Insert(new ExecuteLog
                        //{
                        //    CommandId = Command.Id,
                        //    CreationTime = DateTime.Now,
                        //    LogType = "INFO",
                        //    Message = "Build version already exists.",
                        //    NodeId = AgentConsts.AgentId,
                        //    NodeIp = NodeStatus.GetIpAddress(),
                        //    TaskId = Argument.TaskId.ToString()
                        //});
                    }
                    //ExecuteLog.Insert(new ExecuteLog
                    //{
                    //    CommandId = Command.Id,
                    //    CreationTime = DateTime.Now,
                    //    LogType = "INFO",
                    //    Message = "Task start running...",
                    //    NodeId = AgentConsts.AgentId,
                    //    NodeIp = NodeStatus.GetIpAddress(),
                    //    TaskId = Argument.TaskId.ToString()
                    //});
                    HandlerFactory.AddRunningCount();

                    if (Argument.Entry.ToLower().EndsWith(".exe"))
                    {
                        var entry = Path.Combine(publishTargetDirectory, Argument.Entry);
                        ProcessUtil.Execute(Argument.TaskId, Command.Id, entry, Argument.ExecuteArguments,
                            publishTargetDirectory);
                    }
                    else
                    {
                        ProcessUtil.Execute(Argument.TaskId, Command.Id, "dotnet", $"\"{Argument.Entry} {Argument.ExecuteArguments}\" ", publishTargetDirectory);
                    }

                    //ExecuteLog.Insert(new ExecuteLog
                    //{
                    //    CommandId = Command.Id,
                    //    CreationTime = DateTime.Now,
                    //    LogType = "INFO",
                    //    Message = "Task finished.",
                    //    NodeId = AgentConsts.AgentId,
                    //    NodeIp = NodeStatus.GetIpAddress(),
                    //    TaskId = Argument.TaskId.ToString()
                    //});
                    HandlerFactory.ReduceRunningCount();
                }
                catch (Exception e)
                {
                    
                }
            });
        }

        public void Test()
        {
            var key = new ProjectKey
            {
                SolutionId = 2,
                ProjectName = "DotnetSpider",
                Version = "108f02a84cc13cffb67353316ba992b2b94e5dd3"
            };

            var directory = Path.Combine(AgentConsts.ProjectsDirectory, "DotnetSpider");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var publishTargetDirectory = Path.Combine(directory, "108f02a84cc13cffb67353316ba992b2b94e5dd3", "Test");
            var filePath = Path.Combine(directory, "108f02a84cc13cffb67353316ba992b2b94e5dd3.zip");
            var downloadUrl = Config.Instance.ProjectDownloadUrl;
            var r = GetResponse(downloadUrl, key);
            var bytes = r.Content.ReadAsByteArrayAsync().Result;

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            ZipUtil.UnZip(filePath, publishTargetDirectory);

        }

        private HttpResponseMessage GetResponse(string url, ProjectKey key)
        {
            var httpClient = new HttpClient();
            var data = JsonConvert.SerializeObject(key);
            var byteData = Encoding.UTF8.GetBytes(data);
            using (var s = new MemoryStream(byteData))
            {
                HttpContent c = new StreamContent(s);
                var r = httpClient.PostAsync(url, c).Result;
                try
                {
                    r.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    //ExecuteLog.Insert(new ExecuteLog
                    //{
                    //    CommandId = Command.Id,
                    //    CreationTime = DateTime.Now,
                    //    LogType = "WARN",
                    //    Message = e.Message,
                    //    NodeId = AgentConsts.AgentId,
                    //    NodeIp = NodeStatus.GetIpAddress(),
                    //    TaskId = Argument.TaskId.ToString()
                    //});
                    return null;
                }

                return r;
            }
        }

        public class ProjectKey
        {
            public int SolutionId { get; set; }
            public string ProjectName { get; set; }
            public string Version { get; set; }
        }
    }
}