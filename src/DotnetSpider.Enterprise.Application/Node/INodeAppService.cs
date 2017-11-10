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
		void EnableNode(string nodeId);
		void DisableNode(string nodeId);
		List<MessageOutputDto> Heartbeat(NodeHeartbeatInputDto input);
		List<NodeOutputDto> QueryNodes(int page, int pageSize, string sort);
	}
}
