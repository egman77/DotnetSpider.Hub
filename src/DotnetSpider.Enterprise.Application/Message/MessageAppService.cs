using AutoMapper;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotnetSpider.Enterprise.Application.Message
{
	public class MessageAppService : AppServiceBase, IMessageAppService
	{
		public MessageAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
		}

		public void Add(AddMessageInputDto input)
		{
			var message = Mapper.Map<Domain.Entities.Message>(input);
			DbContext.Message.Add(message);
			DbContext.SaveChanges();
			Logger.LogWarning($"Add message {input}.");
		}

		public void AddRange(List<AddMessageInputDto> input)
		{
			var messages = Mapper.Map<List<Domain.Entities.Message>>(input);
			DbContext.Message.AddRange(messages);
			DbContext.SaveChanges();
			Logger.LogWarning($"Add messages {input}.");
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
			foreach (var message in messages)
			{
				Logger.LogWarning($"Consume message: {JsonConvert.SerializeObject(message)}.");
			}
			return Mapper.Map<List<MessageOutputDto>>(messages);
		}
	}
}
