using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using DotnetSpider.Hub.Application.Message;
using DotnetSpider.Hub.Application.Message.Dtos;
using DotnetSpider.Hub.Application.Node.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetSpider.Hub.Application.Node
{
	public class NodeAppService : AppServiceBase, INodeAppService
	{
		private readonly IMessageAppService _messageAppService;

		public NodeAppService(ApplicationDbContext dbcontext, IMessageAppService messageAppService,
			ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, userManager)
		{
			_messageAppService = messageAppService;
		}

		public void Enable(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				Logger.Information($"Enable node {nodeId}.");
				node.IsEnable = true;
				DbContext.SaveChanges();
			}
			else
			{
				Logger.Information($"Node {nodeId} unfound.");
			}
		}

		public void Disable(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				Logger.Information($"Disable node {nodeId}.");
				node.IsEnable = false;
				DbContext.SaveChanges();
			}
			else
			{
				Logger.Information($"Node {nodeId} unfound.");
			}
		}

		public PaginationQueryDto Query(PaginationQueryInput input)
		{
			if (input == null)
			{
				throw new DotnetSpiderHubException($"{nameof(input)} should not be null.");
			}

			PaginationQueryDto output;
			switch (input.Sort?.ToLower())
			{
				case "enable":
					{
						output = DbContext.Node.PageList<Core.Entities.Node, long, bool>(input, null, d => d.IsEnable);
						break;
					}
				case "nodeid":
					{
						output = DbContext.Node.PageList<Core.Entities.Node, long, string>(input, null, d => d.NodeId);
						break;
					}
				case "createtime":
					{
						output = DbContext.Node.PageList<Core.Entities.Node, long, DateTime>(input, null, d => d.CreationTime);
						break;
					}
				case "type":
					{
						output = DbContext.Node.PageList<Core.Entities.Node, long, string>(input, null, d => d.Type);
						break;
					}
				default:
					{
						output = DbContext.Node.PageList<Core.Entities.Node, long, bool>(input, null, d => d.IsOnline);
						break;
					}
			}

			List<NodeDto> nodeOutputs = new List<NodeDto>();
			var nodes = (List<Core.Entities.Node>)output.Result;

			foreach (var node in nodes)
			{
				var nodeOutput = new NodeDto();
				nodeOutput.CreationTime = node.CreationTime;
				nodeOutput.IsEnable = node.IsEnable;
				nodeOutput.NodeId = node.NodeId;
				nodeOutput.IsOnline = IsOnlineNode(node);

				if (nodeOutput.IsOnline)
				{
					var lastHeartbeat = DbContext.NodeHeartbeat.OrderByDescending(t => t.Id)
						.FirstOrDefault(h => h.NodeId == node.NodeId);
					nodeOutput.CPULoad = lastHeartbeat.CpuLoad;
					nodeOutput.FreeMemory = lastHeartbeat.FreeMemory;
					nodeOutput.Ip = lastHeartbeat.Ip;
					nodeOutput.Os = lastHeartbeat.Os;
					nodeOutput.ProcessCount = lastHeartbeat.ProcessCount;
					nodeOutput.Type = lastHeartbeat.Type;
					nodeOutput.CPUCoreCount = lastHeartbeat.CpuCoreCount;
					nodeOutput.TotalMemory = lastHeartbeat.TotalMemory;
					nodeOutput.Version = lastHeartbeat.Version;
				}
				else
				{
					nodeOutput.CPULoad = 0;
					nodeOutput.FreeMemory = 0;
					nodeOutput.Ip = "unkonw";
					nodeOutput.Os = "unkonw";
					nodeOutput.ProcessCount = 0;
					nodeOutput.CPUCoreCount = 0;
					nodeOutput.TotalMemory = 0;
					nodeOutput.Type = "default";
					nodeOutput.Version = "unkonw";
				}

				nodeOutputs.Add(nodeOutput);
			}

			output.Result = nodeOutputs;
			return output;
		}

		public List<NodeDto> GetAvailable(string os, string type, int nodeCount)
		{
			List<Core.Entities.Node> nodes;
			var compareTime = DateTime.Now.AddSeconds(-60);

			if (string.IsNullOrEmpty(os) || "all" == os.ToLower())
			{
				nodes = DbContext.Node.Where(a =>
					a.IsEnable && a.IsOnline && a.Type == type && a.LastModificationTime > compareTime).ToList();
			}
			else
			{
				nodes = DbContext.Node.Where(a =>
					a.IsEnable && a.IsOnline && a.Os.Contains(os) && a.Type == type &&
					a.LastModificationTime > compareTime).ToList();
			}

			var availableNodes = new List<Core.Entities.Node>();
			foreach (var node in nodes)
			{
				var heartbeat = DbContext.NodeHeartbeat.OrderByDescending(a => a.Id)
					.FirstOrDefault(a => a.NodeId == node.NodeId);
				if (heartbeat.CpuLoad < 90 && heartbeat.FreeMemory > 150)
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
					var newList = new List<Core.Entities.Node>();
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

		private bool IsOnlineNode(Core.Entities.Node node)
		{
			if (node.LastModificationTime != null)
			{
				var value = (DateTime.Now - node.LastModificationTime).Value;
				return value.TotalSeconds < 60;
			}

			return false;
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
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				var message = new CreateMessageInput
				{
					ApplicationName = "NULL",
					Name = Core.Entities.Message.ExitMessageName,
					NodeId = nodeId,
					TaskId = string.Empty
				};
				Logger.Information($"Exit node: {nodeId}.");
				_messageAppService.Create(message);
			}
			else
			{
				throw new DotnetSpiderHubException("Node unfound.");
			}
		}

		public void Delete(string nodeId)
		{
			var node = DbContext.Node.FirstOrDefault(n => n.NodeId == nodeId);
			if (node != null)
			{
				var nodeIdParameter = new SqlParameter("NodeId", nodeId);
				DbContext.Database.ExecuteSqlCommand($"DELETE FROM NodeHeartbeat WHERE NodeId=@NodeId",
					nodeIdParameter);
				DbContext.Node.Remove(node);
				DbContext.SaveChanges();
				Logger.Information($"Remove node: {nodeId}.");
			}
		}

		public void Control(string nodeId, ActionType action)
		{
			switch (action)
			{
				case ActionType.Disable:
					{
						Disable(nodeId);
						break;
					}
				case ActionType.Enable:
					{
						Enable(nodeId);
						break;
					}
				case ActionType.Exit:
					{
						Exit(nodeId);
						break;
					}
			}
		}
	}
}