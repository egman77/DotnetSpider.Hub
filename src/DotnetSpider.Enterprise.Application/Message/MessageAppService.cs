using AutoMapper;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotnetSpider.Enterprise.Application.Message
{
	public class MessageAppService : AppServiceBase, IMessageAppService
	{
		public MessageAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
		}

		public void Create(CreateMessageInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return;
			}
			var message = Mapper.Map<Core.Entities.Message>(input);
			DbContext.Message.Add(message);
			DbContext.SaveChanges();
			Logger.LogWarning($"Crate message {JsonConvert.SerializeObject(input)} success.");
		}

		public void Create(IEnumerable<CreateMessageInput> input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return;
			}
			var messages = Mapper.Map<List<Core.Entities.Message>>(input);
			DbContext.Message.AddRange(messages);
			DbContext.SaveChanges();
			Logger.LogWarning($"Create messages {JsonConvert.SerializeObject(input)} success.");
		}

		public IEnumerable<MessageDto> Consume(string nodeId)
		{
			var messages = DbContext.Message.Where(m => m.NodeId == nodeId);
			var result = Mapper.Map<List<MessageDto>>(messages);
			var messageHistories = Mapper.Map<List<MessageHistory>>(messages);
			foreach (var messageHistory in messageHistories)
			{
				messageHistory.Id = 0;
			}
			DbContext.MessageHistory.AddRange(messageHistories);
			DbContext.Message.RemoveRange(messages);
			DbContext.SaveChanges();
			if (result.Count > 0)
			{
				Logger.LogWarning($"Consume messages: {JsonConvert.SerializeObject(result)}.");
			}
			return result;
		}
	}
}
