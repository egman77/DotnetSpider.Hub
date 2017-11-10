using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Newtonsoft.Json;
using DotnetSpider.Enterprise.Domain.Entities;
using AutoMapper;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Message.Dto;

namespace DotnetSpider.Enterprise.Application.Node
{
	public class NodeAppService : AppServiceBase, INodeAppService
	{
		private readonly IMessageAppService _messageAppService;

		public NodeAppService(ApplicationDbContext dbcontext, IMessageAppService messageAppService) : base(dbcontext)
		{
			_messageAppService = messageAppService;
		}

		public void EnableNode(string nodeId)
		{
			var node = DbContext.Nodes.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				node.IsEnable = true;
			}
			DbContext.SaveChanges();
		}

		public void DisableNode(string nodeId)
		{
			var node = DbContext.Nodes.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				node.IsEnable = false;
			}
			DbContext.SaveChanges();
		}

		public List<MessageOutputDto> Heartbeat(NodeHeartbeatInputDto input)
		{
			AddHeartbeat(input);
			RefreshOnlineStatus(input.NodeId);
			return _messageAppService.QueryMessages(input.NodeId);
		}

		public List<NodeOutputDto> QueryNodes(int page, int pageSize, string sort)
		{
			return null;
		}

		private void AddHeartbeat(NodeHeartbeatInputDto input)
		{
			var heartbeat = Mapper.Map<NodeHeartbeat>(input);
			DbContext.NodeHeartbeats.Add(heartbeat);
		}

		private void RefreshOnlineStatus(string nodeId)
		{
			var node = DbContext.Nodes.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				node.IsOnline = true;
				node.LastModificationTime = DateTime.Now;
			}
			else
			{
				node = new Domain.Entities.Node();
				node.IsEnable = true;
				node.IsOnline = true;
				node.CreationTime = DateTime.Now;
				node.LastModificationTime = DateTime.Now;
				DbContext.Nodes.Add(node);
			}
		}
	}
}
