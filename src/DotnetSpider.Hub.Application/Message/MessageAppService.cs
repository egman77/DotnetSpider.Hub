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
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, userManager)
		{
		}

		public void Create(CreateMessageInput input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}
			Logger.Warning($"Crate message {JsonConvert.SerializeObject(input)}.");
			var message = Mapper.Map<Core.Entities.Message>(input);
			DbContext.Message.Add(message);
			DbContext.SaveChanges();
		}

		public void Create(IEnumerable<CreateMessageInput> input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}
			Logger.Warning($"Create messages {JsonConvert.SerializeObject(input)}.");
			var messages = Mapper.Map<List<Core.Entities.Message>>(input);
			DbContext.Message.AddRange(messages);
			DbContext.SaveChanges();
		}

		public IEnumerable<MessageDto> Consume(string nodeId)
		{
			var messages = DbContext.Message.Where(m => m.NodeId == nodeId);
			var result = Mapper.Map<List<MessageDto>>(messages);
			if (result.Count == 0)
			{
				Logger.Warning($"No messages to node: {nodeId}");
				return result;
			}
			else
			{
				Logger.Warning($"Consume messages: {JsonConvert.SerializeObject(result)}.");
				var messageHistories = Mapper.Map<List<MessageHistory>>(messages);
				messageHistories.ForEach(m => m.Id = 0);
				DbContext.MessageHistory.AddRange(messageHistories);
				DbContext.Message.RemoveRange(messages);
				DbContext.SaveChanges();
				return result;
			}
		}
	}
}
