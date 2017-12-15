using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.Node;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application
{
	public class TaskUtil
	{
		public static void ExitTask(INodeAppService nodeAppService, IMessageAppService messageAppService, Domain.Entities.Task task, ILogger logger = null)
		{
			var runningNodes = nodeAppService.GetAllOnline();

			var messages = new List<AddMessageInputDto>();
			foreach (var status in runningNodes)
			{
				var msg = new AddMessageInputDto
				{
					ApplicationName = "NULL",
					TaskId = task.Id,
					Name = Domain.Entities.Message.CanleMessageName,
					NodeId = status.NodeId
				};
				logger?.LogWarning($"Add CANCLE message: {JsonConvert.SerializeObject(msg)}.");
				messages.Add(msg);
			}
			messageAppService.AddRange(messages);

			task.IsRunning = false;
			task.NodeRunningCount = 0;
		}
	}
}
