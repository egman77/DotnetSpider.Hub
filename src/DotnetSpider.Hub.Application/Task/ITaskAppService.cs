﻿using DotnetSpider.Hub.Application.Task.Dtos;
using DotnetSpider.Hub.Core;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Application.Task
{
	public interface ITaskAppService
	{
		void Control(long taskId, ActionType type);
		PaginationQueryDto Query(PaginationQueryTaskInput input);
		void Create(CreateTaskInput item);
		void Update(UpdateTaskInput item);
		void Delete(long taskId);
		CreateTaskInput Find(long taskId);

		void Run(long taskId);
		void Exit(long taskId);
		void Disable(long taskId);
		void Enable(long taskId);

		void IncreaseRunning(long taskId);
		void ReduceRunning(long taskId);
		TaskDto GetTask(long taskId);
	}
}
