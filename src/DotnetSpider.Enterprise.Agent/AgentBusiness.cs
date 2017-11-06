using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
    public class AgentBusiness
    {
        private static IConfigurationRoot _configurationRoot;
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        int step = 0;
        HttpClient httpClient = null;
        public AgentBusiness()
        {
            httpClient = new HttpClient();
        }
        /// <summary>
        /// 
        /// </summary>
        public void CheckUniqueness()
        {
            if (File.Exists(AgentConsts.RunningLockPath))
            {
                try
                {
                    File.Delete(AgentConsts.RunningLockPath);
                }
                catch (Exception)
                {
                    Console.WriteLine("Agent is running");
                    Console.WriteLine("Enter any key to exit:");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckConfig()
        {
            string nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
            if (!File.Exists(nlogConfigPath))
            {
                Console.WriteLine("Please set nlog.config.");
                Console.WriteLine("Enter any key to exit:");
                Console.Read();
                return;
            }
            LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigPath);
            _logger = LogManager.GetCurrentClassLogger();

            string configPath = Path.Combine(AppContext.BaseDirectory, "config.ini");
            if (!File.Exists(configPath))
            {
                _logger.Error("Please set config.ini.");
                Console.WriteLine("Enter any key to exit:");
                Console.Read();
                return;
            }

            var builder = new ConfigurationBuilder();
            builder.AddIniFile("config.ini");

            _configurationRoot = builder.Build();

            Config.Load(_configurationRoot);


            _logger.Info($"[{++step}] Read Config Success.");
            //var test = new RunProject();
            //test.Test();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadConfig()
        {
            AgentConsts.LoadConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        public void EnableAgent()
        {
            AgentConsts.IsEnabled = true;//目前都用启用状态
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReadAgentId()
        {
            if (string.IsNullOrEmpty(AgentConsts.AgentId))
            {
                AgentConsts.AgentId = Guid.NewGuid().ToString("N");
                _logger.Info($"[{++step}] Create AgentId Success.");
            }

            _logger.Info($"[{++step}] Read AgentId Success.");
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartListener()
        {
            Task.Factory.StartNew(async () =>
             {
                 _logger.Info($"[{++step}] Start Polling...");
                 await httpClient.GetAsync("http://localhost:5000/node").ContinueWith(
                     (requestTask) =>
                     {
                         HttpResponseMessage response = requestTask.Result;
                     });

                 while (!AgentConsts.IsExited)
                 {
                     //http 轮询  
                     //创建心跳包
                     var status = NodeStatus.Current();
                     var msg = status.ToString();
                     var heartBeat = new HeartBeat
                     {
                         AgentId = AgentConsts.AgentId,
                         CountOfRunningTasks = status.CountOfRunningTasks,
                         CpuLoad = status.CpuLoad,
                         FreeMemory = status.FredMemeroy,
                         Ip = status.Address,
                         IsEnabled = status.IsEnabled,
                         Os = status.Os,
                         Timestamp = status.Timestamp,
                         TotalMemory = status.TotalMemory,
                         Version = NodeStatus.Version
                     };
                     // 创建一个异步GET请求，当请求返回时继续处理
                     var data = JsonConvert.SerializeObject(heartBeat);
                     var byteData = Encoding.UTF8.GetBytes(data);
                     using (var s = new MemoryStream(byteData))
                     {
                         HttpContent c = new StreamContent(s);
                         await httpClient.PostAsync(Config.Instance.NodeServerUrl, c).ContinueWith(
                       async (requestTask) =>
                       {
                           HttpResponseMessage response = requestTask.Result;
                           try
                           {
                               // 确认响应成功，否则抛出异常
                               response.EnsureSuccessStatusCode();
                               // 异步读取响应为字符串
                               await response.Content.ReadAsStringAsync().ContinueWith(
                                            (readTask) =>
                                            {
                                                try
                                                {
                                                    var command = JsonConvert.DeserializeObject<Command[]>(readTask.Result);
                                                    for (int i = 0; i < command.Length; i++)
                                                    {
                                                        switch (command[i].Name)
                                                        {
                                                            case Command.Run:
                                                                {
                                                                    RunProject run = new RunProject(command[i]);
                                                                    run.Handle();
                                                                    break;
                                                                }
                                                            case Command.Enable:
                                                                {
                                                                    EnableHandler active = new EnableHandler(command[i]);
                                                                    active.Handle();
                                                                    break;
                                                                }
                                                            case Command.Disable:
                                                                {
                                                                    DisableHandler active = new DisableHandler(command[i]);
                                                                    active.Handle();
                                                                    break;
                                                                }
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    _logger.Error(e, e.ToString());
                                                }

                                            }
                                            );
                           }
                           catch (HttpRequestException e)
                           {
                               _logger.Error($"[{++step}] Node Server Failed.");
                           }
                       });

                     }
                     Thread.Sleep(5000);

                 }
             });
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExitAgent()
        {
            _logger.Info("Start Exit...");
            AgentConsts.IsExited = true;
            if (AgentConsts.IsExited && File.Exists(AgentConsts.RunningLockPath))
            {
                File.Delete(AgentConsts.RunningLockPath);
            }
            Thread.Sleep(5000);
            _logger.Info("Exit success.");
        }


    }
}
