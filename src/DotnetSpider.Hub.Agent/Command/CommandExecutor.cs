using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Serilog;

namespace DotnetSpider.Hub.Agent.Command
{
	public static class CommandExecutor
	{
		private static readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

		static CommandExecutor()
		{
			Commands.Add(CommandNames.CancelName, new Cancel());//取消
			Commands.Add(CommandNames.ExitName, new Exit());//退出
			Commands.Add(CommandNames.RunName, new Run());//运行
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Execute(Messsage command, AgentClient client)
		{
			if (client.HasExited)
			{
				return;
			}
			if (command.NodeId != Env.NodeId)
			{
				Log.Logger.Error($"NodeId not match: {JsonConvert.SerializeObject(command)}.");
				return;
			}

			Log.Logger.Information($"Consume message: {JsonConvert.SerializeObject(command)}.");

			try
			{
				if (Commands.ContainsKey(command.Name))
				{

					Commands[command.Name].Execute(command, client);
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Execute command {JsonConvert.SerializeObject(command)} failed: {e}");
			}
		}
	}
}
