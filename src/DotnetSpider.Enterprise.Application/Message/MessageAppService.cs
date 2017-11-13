using AutoMapper;
using DotnetSpider.Enterprise.Application.Message.Dto;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Message
{
	public class MessageAppService : AppServiceBase, IMessageAppService
	{
		public MessageAppService(ApplicationDbContext dbcontext) : base(dbcontext)
		{
		}

		public List<MessageOutputDto> QueryMessages(string nodeId)
		{
			var messages = DbContext.Message.Where(m => m.NodeId == nodeId).ToList();
			var messageHistories = Mapper.Map<List<MessageHistory>>(messages);
			DbContext.MessageHistory.AddRange(messageHistories);
			DbContext.SaveChanges();
			return Mapper.Map<List<MessageOutputDto>>(messages);
		}
	}
}
