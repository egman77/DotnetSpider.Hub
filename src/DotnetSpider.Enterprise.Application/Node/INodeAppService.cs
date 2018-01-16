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
		List<MessageDto> Heartbeat(NodeHeartbeatInput input);
		PaginationQueryDto Query(PaginationQueryInput input);
		List<NodeDto> GetAvailable(string os, int type, int nodeCount);
		List<NodeDto> GetAllOnline();
		void Exit(string nodeId);
		void Remove(string nodeId);
		int GetOnlineNodeCount();
	}
}
