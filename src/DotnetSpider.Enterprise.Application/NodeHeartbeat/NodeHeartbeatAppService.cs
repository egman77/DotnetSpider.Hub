using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DotnetSpider.Enterprise.Application.NodeHeartbeat.Dto;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Core.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Application.NodeHeartbeat
{
	public class NodeHeartbeatAppService : AppServiceBase, INodeHeartbeatAppService
	{
		private readonly IMessageAppService _messageAppService;

		public NodeHeartbeatAppService(IMessageAppService messageAppService, 
			ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory) : base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_messageAppService = messageAppService;
		}

		public IEnumerable<NodeHeartbeatOutput> Create(NodeHeartbeatInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return Enumerable.Empty<NodeHeartbeatOutput>();
			}

			CreateHeartbeat(input);
			RefreshOnlineStatus(input);
			DbContext.SaveChanges();
			var messages = _messageAppService.Consume(input.NodeId);
			return Mapper.Map<IEnumerable<NodeHeartbeatOutput>>(messages);
		}


		private void CreateHeartbeat(NodeHeartbeatInput input)
		{
			var heartbeat = Mapper.Map<Core.Entities.NodeHeartbeat>(input);
			DbContext.NodeHeartbeat.Add(heartbeat);
		}

		private void RefreshOnlineStatus(NodeHeartbeatInput input)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == input.NodeId);
			if (node != null)
			{
				node.IsOnline = true;
				node.Type = input.Type;
				node.Os = input.Os;
				node.LastModificationTime = DateTime.Now;
			}
			else
			{
				node = new Core.Entities.Node();
				node.NodeId = input.NodeId;
				node.IsEnable = true;
				node.IsOnline = true;
				node.CreationTime = DateTime.Now;
				node.Type = input.Type;
				node.Os = input.Os;
				node.LastModificationTime = node.CreationTime;
				DbContext.Node.Add(node);
			}
		}
	}
}
