using System.Collections.Generic;
using DotnetSpider.Enterprise.Application.Message.Dtos;

namespace DotnetSpider.Enterprise.Application.Message
{
	public interface IMessageAppService
	{
		List<MessageOutputDto> QueryMessages(string nodeId);
		void AddRange(List<AddMessageInputDto> messages);
		void Add(AddMessageInputDto message);
	}
}
