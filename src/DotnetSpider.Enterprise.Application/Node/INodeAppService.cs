using System.Collections.Generic;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Node
{
	public interface INodeAppService
	{
		void Enable(string nodeId);
		void Disable(string nodeId);
		List<MessageOutputDto> Heartbeat(NodeHeartbeatInputDto input);
		PagingQueryOutputDto Query(PagingQueryInputDto input);
		List<NodeOutputDto> GetAvailable(string os, int type, int nodeCount);
		List<NodeOutputDto> GetAllOnline();
		void Exit(string nodeId);
		void Remove(string nodeId);
		int GetOnlineNodeCount();
	}
}
