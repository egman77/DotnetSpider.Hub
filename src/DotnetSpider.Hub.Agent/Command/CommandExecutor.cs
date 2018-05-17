using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NLog;

namespace DotnetSpider.Hub.Agent.Command
{
	public static class CommandExecutor
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private static readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

		static CommandExecutor()
		{
			Commands.Add(CommandNames.CancelName, new Cancel());
			Commands.Add(CommandNames.ExitName, new Exit());
			Commands.Add(CommandNames.RunName, new Run());
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
				Logger.Error($"Command error: {JsonConvert.SerializeObject(command)}.");
				return;
			}

			Logger.Info($"Consume message: {JsonConvert.SerializeObject(command)}.");

			try
			{
				if (Commands.ContainsKey(command.Name))
				{
					Commands[command.Name].Execute(command, client);
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Execute command {JsonConvert.SerializeObject(command)} failed: {e}");
			}
		}


	}
}
