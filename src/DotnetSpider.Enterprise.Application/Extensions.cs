using DotnetSpider.Enterprise.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace DotnetSpider.Enterprise.Application
{
	public static class Extensions
	{
		public static PagingQueryOutputDto PageList<T, TKey>(this DbSet<T> dbSet,
			PagingQueryInputDto input, Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderyBy) where T : Entity<long>
		{
			input.Init();
			PagingQueryOutputDto output = new PagingQueryOutputDto();
			IQueryable<T> entities = dbSet.AsQueryable<T>();
			if (where != null)
			{
				entities = entities.Where(where);
			}
			output.Total = entities.Count();

			if (orderyBy == null)
			{
				entities = entities.OrderBy(t => t.Id).Skip((input.Page - 1) * input.Size).Take(input.Size);
			}
			else
			{
				if (input.SortByDesc())
				{
					entities = entities.OrderByDescending(orderyBy).Skip((input.Page - 1) * input.Size).Take(input.Size);
				}
				else
				{
					entities = entities.OrderBy(orderyBy).Skip((input.Page - 1) * input.Size).Take(input.Size);
				}
			}
			output.Page = input.Page;
			output.Size = input.Size;
			output.Result = entities.ToList();
			return output;
		}
	}
}
