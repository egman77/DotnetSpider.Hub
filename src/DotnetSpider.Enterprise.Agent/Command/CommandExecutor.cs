using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotnetSpider.Enterprise.Agent.Command
{
	public static class CommandExecutor
	{
		private readonly static ILogger Logger = LogManager.GetCurrentClassLogger();
		private static readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

		static CommandExecutor()
		{
			_commands.Add(CommandNames.CancelName, new Cancel());
			_commands.Add(CommandNames.ExitName, new Exit());
			_commands.Add(CommandNames.RunName, new Run());
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
				if (_commands.ContainsKey(command.Name))
				{
					_commands[command.Name].Execute(command, client);
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Execute command {JsonConvert.SerializeObject(command)} failed: {e}");
			}
		}


	}
}
