using System.Collections.Generic;
using DotnetSpider.Hub.Application.Message.Dtos;

namespace DotnetSpider.Hub.Application.Message
{
	public interface IMessageAppService
	{
		IEnumerable<MessageDto> Consume(string nodeId);
		void Create(IEnumerable<CreateMessageInput> messages);
		void Create(CreateMessageInput message);
	}
}
