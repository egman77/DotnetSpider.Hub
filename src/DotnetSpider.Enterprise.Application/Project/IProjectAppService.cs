using DotnetSpider.Enterprise.Application.Project.Dtos;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Project
{
	public interface IProjectAppService
	{
		void AddProjcet(ModifyProjectDto item);
		void RemoveProject(int projectId);
		void ModifyProject(ModifyProjectDto item);
		List<ModifyProjectDto> ListProject();
		List<ProjectDto> GetAll();
		StatusDto GetStatusAndLogs(long buildId);
		void EnableProject(int projectId);
		void DisableProject(int projectId);
	}
}
