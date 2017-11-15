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
using DotnetSpider.Enterprise.Application.Message.Dtos;

namespace DotnetSpider.Enterprise.Application.Node
{
	public class NodeAppService : AppServiceBase, INodeAppService
	{
		private readonly IMessageAppService _messageAppService;

		public NodeAppService(ApplicationDbContext dbcontext, IMessageAppService messageAppService) : base(dbcontext)
		{
			_messageAppService = messageAppService;
		}

		public void Enable(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				node.IsEnable = true;
			}
			DbContext.SaveChanges();
		}

		public void Disable(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
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
			DbContext.SaveChanges();
			return _messageAppService.QueryMessages(input.NodeId);
		}

		public PagingQueryOutputDto QueryNodes(PagingQueryInputDto input)
		{
			PagingQueryOutputDto output = new PagingQueryOutputDto();
			switch (input.SortKey)
			{
				case "enable":
					{
						output = DbContext.Node.PageList(input, null, d => d.IsEnable);
						break;
					}
				case "nodeid":
					{
						output = DbContext.Node.PageList(input, null, d => d.NodeId);
						break;
					}
				case "createtime":
					{
						output = DbContext.Node.PageList(input, null, d => d.CreationTime);
						break;
					}
				default:
					{
						output = DbContext.Node.PageList(input, null, d => d.IsOnline);
						break;
					}
			}
			List<NodeOutputDto> nodeOutputs = new List<NodeOutputDto>();
			var nodes = output.Result as List<Domain.Entities.Node>;
			var timeoutHeartbeat = DateTime.Now.AddMinutes(-1);
			foreach (var node in nodes)
			{
				var nodeOutput = new NodeOutputDto();
				nodeOutput.CreationTime = node.CreationTime;
				nodeOutput.IsEnable = node.IsEnable;
				nodeOutput.NodeId = node.NodeId;
				var lastHeartbeat = DbContext.NodeHeartbeat.FirstOrDefault(h => h.NodeId == node.NodeId && h.CreationTime > timeoutHeartbeat);
				nodeOutput.IsOnline = lastHeartbeat == null ? false : true;
				if (lastHeartbeat != null)
				{
					nodeOutput.CPULoad = lastHeartbeat.CPULoad;
					nodeOutput.FreeMemory = lastHeartbeat.FreeMemory;
					nodeOutput.Ip = lastHeartbeat.Ip;
					nodeOutput.Os = lastHeartbeat.Os;
					nodeOutput.ProcessCount = lastHeartbeat.ProcessCount;
					nodeOutput.CPUCoreCount = lastHeartbeat.CPUCoreCount;
					nodeOutput.TotalMemory = lastHeartbeat.TotalMemory;
					nodeOutput.Version = lastHeartbeat.Version;
				}
				else
				{
					nodeOutput.CPULoad = 0;
					nodeOutput.FreeMemory = 0;
					nodeOutput.Ip = "UNKONW";
					nodeOutput.Os = "UNKONW";
					nodeOutput.ProcessCount = 0;
					nodeOutput.CPUCoreCount = 0;
					nodeOutput.TotalMemory = 0;
					nodeOutput.Version = "UNKONW";
				}
				nodeOutputs.Add(nodeOutput);
			}
			output.Result = nodeOutputs;
			return output;
		}

		public List<NodeOutputDto> GetAvailableNodes(string os, int nodeCount)
		{
			List<Domain.Entities.Node> nodes = null;
			if (string.IsNullOrEmpty(os) || "all" == os.ToLower())
			{
				nodes = DbContext.Node.Where(a => a.IsEnable && a.IsOnline).ToList();
			}
			else
			{
				nodes = DbContext.Node.Where(a => a.IsEnable && a.IsOnline && a.Os.Contains(os)).ToList();
			}
			var nodeScores = new Dictionary<Domain.Entities.Node, int>();
			foreach (var node in nodes)
			{
				var heartbeat = DbContext.NodeHeartbeat.Where(a => a.NodeId == node.NodeId).OrderByDescending(a => a.CreationTime).FirstOrDefault();
				var score = 0;
				if ((DateTime.Now - heartbeat.CreationTime).TotalSeconds < 120)
				{
					if (heartbeat.ProcessCount < 1)
					{
						score = 5;
					}
					else if (heartbeat.ProcessCount == 1)
					{
						score = 2;
					}
					else
					{
						score = 0;
					}

					if (heartbeat.FreeMemory >= 800)
					{
						score += 3;
					}
					else if (heartbeat.FreeMemory >= 500)
					{
						score += 1;
					}
					if (heartbeat.CPULoad < 30)
					{
						score += 2;
					}
					else if (heartbeat.CPULoad < 50)
					{
						score += 1;
					}
					nodeScores.Add(node, score);
				}
			}

			var availableNodes = nodeScores.OrderByDescending(a => a.Value).Select(n => n.Key).ToList();
			var resultNodes = availableNodes.Count > nodeCount ? Mapper.Map<List<NodeOutputDto>>(availableNodes.Take(nodeCount)) : Mapper.Map<List<NodeOutputDto>>(availableNodes);
			return resultNodes;
		}

		private void AddHeartbeat(NodeHeartbeatInputDto input)
		{
			var heartbeat = Mapper.Map<NodeHeartbeat>(input);
			DbContext.NodeHeartbeat.Add(heartbeat);
		}

		private void RefreshOnlineStatus(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				node.IsOnline = true;
				node.LastModificationTime = DateTime.Now;
			}
			else
			{
				node = new Domain.Entities.Node();
				node.NodeId = nodeId;
				node.IsEnable = true;
				node.IsOnline = true;
				node.CreationTime = DateTime.Now;
				node.LastModificationTime = DateTime.Now;
				DbContext.Node.Add(node);
			}
		}

		public List<NodeOutputDto> GetAllOnlineNodes()
		{
			var nodes = DbContext.Node.Where(a => a.IsEnable && a.IsOnline).ToList();
			return Mapper.Map<List<NodeOutputDto>>(nodes);
		}

		public void Exit(string nodeId)
		{
			var message = new AddMessageInputDto
			{
				ApplicationName = "NULL",
				Name = Domain.Entities.Message.ExitMessageName,
				NodeId = nodeId,
				TaskId = 0
			};
			_messageAppService.Add(message);
		}
	}
}
