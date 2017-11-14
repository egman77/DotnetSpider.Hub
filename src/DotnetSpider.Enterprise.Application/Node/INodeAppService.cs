using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.Application.Message.Dto;

namespace DotnetSpider.Enterprise.Application.Node
{
	public interface INodeAppService
	{
		void Enable(string nodeId);
		void Disable(string nodeId);
		List<MessageOutputDto> Heartbeat(NodeHeartbeatInputDto input);
		PagingQueryOutputDto QueryNodes(PagingQueryInputDto input);
		List<NodeOutputDto> GetAvailableNodes(string os,int nodeCount);
		List<NodeOutputDto> GetAllOnlineNodes();
	}
}
