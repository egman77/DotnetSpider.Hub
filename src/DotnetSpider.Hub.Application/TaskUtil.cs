using System.Collections.Generic;
using DotnetSpider.Hub.Application.Message;
using DotnetSpider.Hub.Application.Message.Dtos;
using DotnetSpider.Hub.Application.Node;
using Newtonsoft.Json;
using Serilog;

namespace DotnetSpider.Hub.Application
{
	public class TaskUtil
	{
		public static void ExitTask(INodeAppService nodeAppService, IMessageAppService messageAppService, Core.Entities.Task task, ILogger logger = null)
		{
			var runningNodes = nodeAppService.GetAllOnline();

			var messages = new List<CreateMessageInput>();
			foreach (var status in runningNodes)
			{
				var msg = new CreateMessageInput
				{
					ApplicationName = "NULL",
					TaskId = task.Id,
					Name = Core.Entities.Message.CanleMessageName,
					NodeId = status.NodeId
				};
				logger?.Warning($"Add CANCLE message: {JsonConvert.SerializeObject(msg)}.");
				messages.Add(msg);
			}
			messageAppService.Create(messages);

			task.IsRunning = false;
			task.NodeRunningCount = 0;
		}
	}
}
