using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DotnetSpider.Hub.Application.Message;
using DotnetSpider.Hub.Application.Node;
using DotnetSpider.Hub.Application.NodeHeartbeat.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Hub.Application.NodeHeartbeat
{
	public class NodeHeartbeatAppService : AppServiceBase, INodeHeartbeatAppService
	{
		private readonly IMessageAppService _messageAppService;

		public NodeHeartbeatAppService(IMessageAppService messageAppService,
			ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, userManager)
		{
			_messageAppService = messageAppService;
		}

		public IEnumerable<NodeHeartbeatOutput> Create(NodeHeartbeatInput input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
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
