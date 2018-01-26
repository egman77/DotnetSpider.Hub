using System.Collections.Generic;
using DotnetSpider.Enterprise.Application.Message.Dtos;

namespace DotnetSpider.Enterprise.Application.Message
{
	public interface IMessageAppService
	{
		IEnumerable<MessageDto> Consume(string nodeId);
		void Create(IEnumerable<CreateMessageInput> messages);
		void Create(CreateMessageInput message);
	}
}
