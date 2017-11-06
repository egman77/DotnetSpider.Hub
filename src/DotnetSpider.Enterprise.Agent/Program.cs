using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Config;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
    public class Program
    {

        static void Main(string[] args)
        {
            Console.Title = $"DotnetSpider.Enterprise.ServerAgent v{NodeStatus.Version}";
            var agent = new AgentBusiness();
            agent.CheckUniqueness();
            using (File.Create(AgentConsts.RunningLockPath))
            {                
                agent.CheckConfig();
                agent.LoadConfig();
                agent.EnableAgent();
                agent.ReadAgentId();
                agent.StartListener();
                Thread.Sleep(5000);

                Console.WriteLine("Enter q: to exit:");

                while (Console.ReadLine() != "q:")
                {
                    Console.WriteLine("Press q: to exit.");
                }
            }
            agent.ExitAgent();
        }

    }
}
