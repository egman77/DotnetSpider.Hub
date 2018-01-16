using System.Collections.Generic;
using DotnetSpider.Enterprise.Application.Message.Dtos;

namespace DotnetSpider.Enterprise.Application.Message
{
	public interface IMessageAppService
	{
		List<MessageDto> Query(string nodeId);
		void Add(IEnumerable<AddMessageInput> messages);
		void Add(AddMessageInput message);
	}
}
