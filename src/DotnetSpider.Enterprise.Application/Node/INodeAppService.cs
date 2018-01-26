using System.Collections.Generic;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Node
{
	public interface INodeAppService
	{
		void Enable(string nodeId);
		void Disable(string nodeId);
		PaginationQueryDto Query(PaginationQueryInput input);
		List<NodeDto> GetAvailable(string os, int type, int nodeCount);
		List<NodeDto> GetAllOnline();
		void Exit(string nodeId);
		void Delete(string nodeId);
		void Control(string nodeId, ActionType action);
		int GetOnlineNodeCount();
	}
}
