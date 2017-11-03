using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using System.Linq;
using DotnetSpider.Enterprise.Application.Project.Dtos;
using StackExchange.Redis;
using Newtonsoft.Json;
using DotnetSpider.Enterprise.Application.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;
using System.Linq.Expressions;

namespace DotnetSpider.Enterprise.Application.Project
{
	public class ProjectAppService : AppServiceBase, IProjectAppService
	{
		public ProjectAppService(ApplicationDbContext dbcontext)
			: base(dbcontext)
		{

		}

		public void AddProjcet(ModifyProjectDto item)
		{
			var proj = new Domain.Entities.Project
			{
				Client = item.Client,
				Executive = item.Executive,
				Note = item.Note,
				IsEnabled = item.IsEnabled,
				Name = item.Name,
			};
			DbContext.Projects.Add(proj);
			DbContext.SaveChanges();
			item.Id = proj.Id;
		}

		public List<ModifyProjectDto> ListProject()
		{
			var resultList = new List<ModifyProjectDto>();
			var list = DbContext.Projects.OrderByDescending(a => a.CreationTime).ToList();
			list.ForEach(a => resultList.Add(new ModifyProjectDto
			{
				Id = a.Id,
				Client = a.Client,
				Executive = a.Executive,
				IsEnabled = a.IsEnabled,
				Name = a.Name,
				Note = a.Note
			}));
			return resultList;
		}

		public void ModifyProject(ModifyProjectDto item)
		{
			var project = DbContext.Projects.FirstOrDefault(a => a.Id == item.Id);
			project.Note = item.Note;
			project.Executive = item.Executive;
			project.Client = item.Client;
			project.Name = item.Name;
			project.IsEnabled = item.IsEnabled;

			DbContext.Projects.Update(project);
			DbContext.SaveChanges();
		}

		public void RemoveProject(int projectId)
		{
			DbContext.DoWithTransaction(() =>
			{
				var project = DbContext.Projects.FirstOrDefault(a => a.Id == projectId);
				if (project != null)
				{
					DbContext.Projects.Remove(project);
					var tasks = DbContext.Tasks.Where(t => t.ProjectId == projectId);
					DbContext.Tasks.RemoveRange(tasks);
				}
			});
		}

		public List<ProjectDto> GetAll()
		{
			var results = DbContext.Projects.ToList();
			return AutoMapper.Mapper.Map<List<ProjectDto>>(results);
		}

		public StatusDto GetStatusAndLogs(long buildId)
		{
			throw new NotSupportedException();
			//var statusItem = DbContext.Build_Statuses.FirstOrDefault(a => a.Id == buildId);
			//if (statusItem != null)
			//{
			//	var logs = DbContext.Build_Logs
			//		.Where(a => a.BuildId == statusItem.Id)
			//		.OrderByDescending(a => a.CreationTime)
			//		.ToList();

			//	return new StatusDto
			//	{
			//		BuildId = statusItem.Id,
			//		Logs = AutoMapper.Mapper.Map<List<BuildLogDto>>(logs),
			//		Status = statusItem.Status,
			//		FilePath = statusItem.FilePath
			//	};
			//}
			//return null;
		}

		public void EnableProject(int projectId)
		{
			var project = DbContext.Projects.FirstOrDefault(a => a.Id == projectId);
			if (project != null && !project.IsEnabled)
			{
				project.IsEnabled = true;
				DbContext.Update(project);
				DbContext.SaveChanges();
			}
		}

		public void DisableProject(int projectId)
		{
			var project = DbContext.Projects.FirstOrDefault(a => a.Id == projectId);
			if (project != null && project.IsEnabled)
			{
				project.IsEnabled = false;
				DbContext.Update(project);
				DbContext.SaveChanges();
			}
		}
	}
}
