using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DotnetSpider.Hub.Application.Message.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DotnetSpider.Hub.Application.Message
{
	public class MessageAppService : AppServiceBase, IMessageAppService
	{
		public MessageAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
		}

		public void Create(CreateMessageInput input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}
			var message = Mapper.Map<Hub.Core.Entities.Message>(input);
			DbContext.Message.Add(message);
			DbContext.SaveChanges();
			Logger.Warning($"Crate message {JsonConvert.SerializeObject(input)} success.");
		}

		public void Create(IEnumerable<CreateMessageInput> input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}
			var messages = Mapper.Map<List<Hub.Core.Entities.Message>>(input);
			DbContext.Message.AddRange(messages);
			DbContext.SaveChanges();
			Logger.Warning($"Create messages {JsonConvert.SerializeObject(input)} success.");
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
				Logger.Warning($"Consume messages: {JsonConvert.SerializeObject(result)}.");
			}
			return result;
		}
	}
}
