using System.Collections.Generic;
using DotnetSpider.Hub.Application.NodeHeartbeat.Dtos;

namespace DotnetSpider.Hub.Application.NodeHeartbeat
{
	public interface INodeHeartbeatAppService
	{
		IEnumerable<NodeHeartbeatOutput> Create(NodeHeartbeatInput input);
	}
}
