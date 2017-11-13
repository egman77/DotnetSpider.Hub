using DotnetSpider.Enterprise.Application.Message.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Message
{
	public interface IMessageAppService
	{
		List<MessageOutputDto> QueryMessages(string nodeId);
	}
}
