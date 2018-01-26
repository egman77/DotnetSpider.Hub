using DotnetSpider.Enterprise.Application.NodeHeartbeat.Dto;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application.NodeHeartbeat
{
	public interface INodeHeartbeatAppService
	{
		IEnumerable<NodeHeartbeatOutput> Create(NodeHeartbeatInput input);
	}
}
