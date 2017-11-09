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

namespace DotnetSpider.Enterprise.Application.Node
{
	public class NodeAppService : AppServiceBase, INodeAppService
	{
		public NodeAppService(ApplicationDbContext dbcontext) : base(dbcontext)
		{
		}

		public List<NodeInfoDto> GetCurrentNodeInfo()
		{
			//var redisEntries = AgentNodesDb.HashGetAll("SERVERS");
			var result = new List<NodeInfoDto>();

			//foreach (var redisEntry in redisEntries)
			//{
			//	var value = redisEntry.Value.ToString();
			//	var splitValue = value.Split('|');

			//	var info = new NodeInfoDto
			//	{
			//		AgentId = redisEntry.Name.ToString(),
			//		Ip = splitValue[0],
			//		CpuLoad = splitValue[1],
			//		FreeMemory = splitValue[2],
			//		TotalMemory = splitValue[3],
			//		Timestamp = splitValue[4],
			//		IsEnabled = splitValue[5],
			//		CountOfRunningTasks = splitValue[6],
			//		Version = splitValue[7],
			//		Os = splitValue[8]
			//	};
			//	var ticks = long.Parse(splitValue[4]);
			//	var interval = (DateTime.UtcNow.Ticks - ticks) / 10000000;
			//	if (interval > 15)
			//	{
			//		info.IsOnline = "False";
			//		info.CpuLoad = string.Empty;
			//		info.FreeMemory = string.Empty;
			//		info.CountOfRunningTasks = string.Empty;
			//	}
			//	else
			//	{
			//		info.IsOnline = "True";
			//	}

			//	result.Add(info);
			//}
			return result;
		}

		public NodeDetailDto GetNodeDetail(string id)
		{
			//if (string.IsNullOrEmpty(id))
			//{
			//	throw new AppException("Agent node id is not correct.");
			//}
			//var heartBeats = DbContext.Agent_HeartBeats.Where(h => h.AgentId == id).OrderByDescending(h => h.Id).Take(100).ToList();
			//if (heartBeats == null || heartBeats.Count == 0)
			//{
			//	throw new AppException("Agent node id is not correct.");
			//}

			var r = new NodeDetailDto();
			//var currentInfoStr = AgentNodesDb.HashGet("SERVERS", id).ToString();
			//var splitInfo = currentInfoStr.Split('|');
			//r.Ip = splitInfo[0];
			//r.IsEnabled = bool.Parse(splitInfo[5]);
			//r.Os = splitInfo[8];
			//r.Version = splitInfo[7];

			//var ticks = long.Parse(splitInfo[4]);
			//var interval = (DateTime.UtcNow.Ticks - ticks) / 10000000;
			//if (interval > 15)
			//{
			//	r.IsOnline = false;
			//}
			//else
			//{
			//	r.IsOnline = true;
			//}

			//var pData = new List<PerformanceData>();
			//var lastTick = ticks;
			//foreach (var heartBeat in heartBeats)
			//{
			//	var lastInterval = (lastTick - heartBeat.Timestamp) / 10000000;
			//	if (lastInterval > 3600)
			//	{
			//		break;
			//	}
			//	lastTick = heartBeat.Timestamp;

			//	var p = new PerformanceData
			//	{
			//		Cpu = heartBeat.CpuLoad,
			//		Memory = Convert.ToInt32(((heartBeat.TotalMemory - heartBeat.FreeMemory) / (decimal)heartBeat.TotalMemory) * (decimal)100),
			//		RunningTasks = heartBeat.CountOfRunningTasks,
			//		Time = new DateTime(heartBeat.Timestamp).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
			//	};
			//	pData.Add(p);
			//}
			//r.PerformanceData = pData;

			return r;
		}

		public NodeEnable EnableNode(NodeEnable input)
		{
			throw new NotSupportedException();
			//var id = input.Id;
			//if (string.IsNullOrEmpty(id))
			//{
			//	throw new AppException("Agent node id is not correct.");
			//}
			//string commandName;
			//if (input.Enable)
			//{
			//	commandName = Command.Enable;
			//}
			//else
			//{
			//	commandName = Command.Disable;
			//}
			//var command = new Command
			//{
			//	Name = commandName,
			//	Id = Guid.NewGuid().ToString("N")
			//};

			//Subscriber.Publish(id, JsonConvert.SerializeObject(command));

			//Thread.Sleep(1000);
			//var value = AgentNodesDb.HashGet("SERVERS", id).ToString();
			//var splitValue = value.Split('|');

			//var isEnabled = bool.Parse(splitValue[5]);
			//return new NodeEnable
			//{
			//	Enable = isEnabled,
			//	Id = id
			//};
		}

		public PagingQueryOutputDto GetLog(GetLogInput input)
		{
			throw new NotSupportedException();
			//var id = input.AgentId;
			//if (string.IsNullOrEmpty(id))
			//{
			//	throw new AppException("Agent node id is not correct.");
			//}

			//input.Init();

			//PagingQueryOutputDto r = null;
			//if (string.IsNullOrEmpty(input.LogLevel))
			//{
			//	r = DbContext.Execute_Logs.PageList(input, l => l.NodeId == id, a => a.CreationTime);
			//}
			//else
			//{
			//	r = DbContext.Execute_Logs.PageList(input, l => l.NodeId == id && l.LogType == input.LogLevel, a => a.CreationTime);
			//}

			//r.Result = Mapper.Map<List<ExecuteLogDto>>(r.Result);

			//return r;
		}

		public List<AgentCommandOutputDto> Heartbeat(NodeHeartbeatInputDto input)
		{
			var heartbeat = Mapper.Map<NodeHeartBeat>(input);
			DbContext.NodeHeartBeats.Add(heartbeat);
			DbContext.SaveChanges();

			return new List<AgentCommandOutputDto>
			{
				new AgentCommandOutputDto
				{
					AngentId="3c8c7065bfe744aabdb9f8e5e11c9ae6",
					Application="dotnet",
					Arguments="Xbjrkj.DataCollection.Startup.dll -s:Xbjrkj.DataCollection.Apps.BaiduSearchSpider -i:guid -tid:1 -a:abcd",
					Name="RUN",
					Task ="test",
					Version="55d21fb6e864b4a0378a2c88e55a72503f4dd9a4"
				}
			};
		}
	}
}
