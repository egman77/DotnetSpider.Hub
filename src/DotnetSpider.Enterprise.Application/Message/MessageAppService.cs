using AutoMapper;
using DotnetSpider.Enterprise.Application.Message.Dto;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;

namespace DotnetSpider.Enterprise.Application.Message
{
	public class MessageAppService : AppServiceBase, IMessageAppService
	{
		public MessageAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration) : base(dbcontext, configuration)
		{
		}

		public void Add(AddMessageInputDto input)
		{
			var message = Mapper.Map<Domain.Entities.Message>(input);
			DbContext.Message.Add(message);
			DbContext.SaveChanges();
		}

		public void AddRange(List<AddMessageInputDto> input)
		{
			var messages = Mapper.Map<List<Domain.Entities.Message>>(input);
			DbContext.Message.AddRange(messages);
			DbContext.SaveChanges();
		}

		public List<MessageOutputDto> QueryMessages(string nodeId)
		{
			var messages = DbContext.Message.Where(m => m.NodeId == nodeId).ToList();
			var messageHistories = Mapper.Map<List<MessageHistory>>(messages);
			foreach (var history in messageHistories)
			{
				history.Id = 0;
			}
			DbContext.MessageHistory.AddRange(messageHistories);
			DbContext.Message.RemoveRange(messages);
			DbContext.SaveChanges();
			return Mapper.Map<List<MessageOutputDto>>(messages);
		}
	}
}
