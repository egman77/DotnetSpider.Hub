using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;

namespace DotnetSpider.Enterprise.Application.Node
{
	public interface INodeAppService
	{
		List<NodeInfoDto> GetCurrentNodeInfo();
		NodeDetailDto GetNodeDetail(string id);
		NodeEnable EnableNode(NodeEnable input);
		PagingQueryOutputDto GetLog(GetLogInput input);
		List<MessageOutputDto> Heartbeat(NodeHeartbeatInputDto input);
	}
}
