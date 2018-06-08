using System.Collections.Generic;
using DotnetSpider.Hub.Application.Node.Dtos;
using DotnetSpider.Hub.Core;

namespace DotnetSpider.Hub.Application.Node
{
	public interface INodeAppService
	{
		void Enable(string nodeId);
		void Disable(string nodeId);
		PaginationQueryDto Query(PaginationQueryInput input);
		List<NodeDto> GetAvailable(string os, string type, int nodeCount);
		List<NodeDto> GetAllOnline();
		void Exit(string nodeId);
		void Delete(string nodeId);
		void Control(string nodeId, ActionType action);
		int GetOnlineNodeCount();
	}
}
