using System;
using System.Collections.Generic;
using System.Linq;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using DotnetSpider.Enterprise.Domain.Entities;
using AutoMapper;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Application.Node
{
	public class NodeAppService : AppServiceBase, INodeAppService
	{
		private readonly IMessageAppService _messageAppService;

		public NodeAppService(ApplicationDbContext dbcontext, IMessageAppService messageAppService, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_messageAppService = messageAppService;
		}

		public void Enable(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				node.IsEnable = true;
				DbContext.SaveChanges();
				Logger.LogInformation($"Enable node {nodeId}.");
			}
		}

		public void Disable(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				node.IsEnable = false;
				DbContext.SaveChanges();
				Logger.LogInformation($"Disable node {nodeId}.");
			}
		}

		public List<MessageDto> Heartbeat(NodeHeartbeatInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return new List<MessageDto>();
			}
			if (IsAuth())
			{
				AddHeartbeat(input);
				RefreshOnlineStatus(input);
				DbContext.SaveChanges();
				return _messageAppService.Query(input.NodeId);
			}
			throw new DotnetSpiderException("Access Denied.");
		}

		public PaginationQueryDto Query(PaginationQueryInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}
			PaginationQueryDto output = new PaginationQueryDto();
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
			List<NodeDto> nodeOutputs = new List<NodeDto>();
			var nodes = output.Result as List<Domain.Entities.Node>;

			foreach (var node in nodes)
			{
				var nodeOutput = new NodeDto();
				nodeOutput.CreationTime = node.CreationTime;
				nodeOutput.IsEnable = node.IsEnable;
				nodeOutput.NodeId = node.NodeId;
				nodeOutput.IsOnline = IsOnlineNode(node);

				if (nodeOutput.IsOnline)
				{
					var lastHeartbeat = DbContext.NodeHeartbeat.OrderByDescending(t => t.Id).FirstOrDefault(h => h.NodeId == node.NodeId);
					nodeOutput.CPULoad = lastHeartbeat.CPULoad;
					nodeOutput.FreeMemory = lastHeartbeat.FreeMemory;
					nodeOutput.Ip = lastHeartbeat.Ip;
					nodeOutput.Os = lastHeartbeat.Os;
					nodeOutput.ProcessCount = lastHeartbeat.ProcessCount;
					nodeOutput.Type = lastHeartbeat.Type;
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
					nodeOutput.Type = 1;
					nodeOutput.Version = "UNKONW";
				}
				nodeOutputs.Add(nodeOutput);
			}
			output.Result = nodeOutputs;
			return output;
		}

		public List<NodeDto> GetAvailable(string os, int type, int nodeCount)
		{
			List<Domain.Entities.Node> nodes = null;
			var compareTime = DateTime.Now.AddSeconds(-60);

			if (string.IsNullOrEmpty(os) || "all" == os.ToLower())
			{
				nodes = DbContext.Node.Where(a => a.IsEnable && a.IsOnline && a.Type == type && a.LastModificationTime > compareTime).ToList();
			}
			else
			{
				nodes = DbContext.Node.Where(a => a.IsEnable && a.IsOnline && a.Os.Contains(os) && a.Type == type && a.LastModificationTime > compareTime).ToList();
			}

			var availableNodes = new List<Domain.Entities.Node>();
			foreach (var node in nodes)
			{
				var heartbeat = DbContext.NodeHeartbeat.OrderByDescending(a => a.Id).FirstOrDefault(a => a.NodeId == node.NodeId);
				if (heartbeat.CPULoad < 90 && heartbeat.FreeMemory > 150)
				{
					availableNodes.Add(node);
				}
			}
			if (availableNodes.Count == 0)
			{
				// TODO SEND REPORT
				return new List<NodeDto>();
			}
			else
			{
				if (availableNodes.Count > nodeCount)
				{
					Random random = new Random();
					var newList = new List<Domain.Entities.Node>();
					foreach (var item in availableNodes)
					{
						newList.Insert(random.Next(newList.Count), item);
					}

					return Mapper.Map<List<NodeDto>>(newList.Take(nodeCount));
				}
				else
				{
					return Mapper.Map<List<NodeDto>>(availableNodes);
				}
			}
		}

		private bool IsOnlineNode(Domain.Entities.Node node)
		{
			var value = (DateTime.Now - node.LastModificationTime).Value;
			return value.TotalSeconds < 60;
		}

		private void AddHeartbeat(NodeHeartbeatInput input)
		{
			var heartbeat = Mapper.Map<NodeHeartbeat>(input);
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
				node = new Domain.Entities.Node();
				node.NodeId = input.NodeId;
				node.IsEnable = true;
				node.IsOnline = true;
				node.CreationTime = DateTime.Now;
				node.Type = input.Type;
				node.Os = input.Os;
				node.LastModificationTime = DateTime.Now;
				DbContext.Node.Add(node);
			}
		}

		public List<NodeDto> GetAllOnline()
		{
			var compareTime = DateTime.Now.AddSeconds(-60);
			var nodes = DbContext.Node.Where(a => a.IsOnline && a.LastModificationTime > compareTime).ToList();
			return Mapper.Map<List<NodeDto>>(nodes);
		}

		public int GetOnlineNodeCount()
		{
			var compareTime = DateTime.Now.AddSeconds(-60);
			return DbContext.Node.Count(a => a.IsOnline && a.LastModificationTime > compareTime);
		}

		public void Exit(string nodeId)
		{
			var message = new AddMessageInput
			{
				ApplicationName = "NULL",
				Name = Domain.Entities.Message.ExitMessageName,
				NodeId = nodeId,
				TaskId = 0
			};
			Logger.LogInformation($"Exit node: {nodeId}.");
			_messageAppService.Add(message);
		}

		public void Remove(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				var nodeIdParameter = new SqlParameter("NodeId", nodeId);
				DbContext.Database.ExecuteSqlCommand($"DELETE FROM NodeHeartbeat WHERE NodeId=@NodeId", nodeIdParameter);
				DbContext.Node.Remove(node);
				DbContext.SaveChanges();
				Logger.LogInformation($"Remove node: {nodeId}.");
			}
		}
	}
}
