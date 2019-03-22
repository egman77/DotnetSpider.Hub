using CommandLine;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading;

namespace DotnetSpider.Hub.Agent
{
	public class Options
	{
        
        /// <summary>
        /// 配置文件
        /// </summary>
		[Option('c', "config", Required = false, HelpText = "Config file")]
		public string Config { get; set; }

        /// <summary>
        /// 是否做为守护进程
        /// </summary>
		[Option('d', "daemon", Required = false, HelpText = "Run as daemon")]
		public bool Daemon { get; set; }
	}

    /// <summary>
    /// 这个是代理节点系统(控制台)
    /// </summary>
	public class Program
	{
		static void Main(string[] args)
		{
            //日志配置
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "{Date}.log"))
				.WriteTo.Console()
				.CreateLogger();


			Parser parser = new Parser(config =>
			{
				config.CaseSensitive = false;//关注敏感的?
				config.EnableDashDash = false; //打开仪表板的仪表板?
				config.CaseInsensitiveEnumValues = false;//关心不敏感的枚举值?
			});

			var result = parser.ParseArguments<Options>(args);
            //如果已解析
			if (result.Tag == ParserResultType.Parsed)
			{
				ThreadPool.SetMaxThreads(256, 256); //线程
				var options = result as Parsed<Options>;
				var agent = new AgentClient(options.Value); //代理客户端
				agent.Run(); //代理运行
			}
			else
			{
                //报告参数错误
				Log.Logger.Error("Arguments uncorrect.");
			}
		}
	}
}
