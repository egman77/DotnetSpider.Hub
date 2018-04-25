using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.EntityFrameworkCore
{
	public static class ApplicationDbContextExtensions
	{
		public static PaginationQueryDto PageList<TEntity, TKey>(this DbSet<TEntity> dbSet, PaginationQueryInput input,
			Expression<Func<TEntity, bool>> where = null,
			Expression<Func<TEntity, TKey>> orderyBy = null) where TEntity : Entity<long>
		{
			input.Validate();
			PaginationQueryDto output = new PaginationQueryDto();
			IQueryable<TEntity> entities = dbSet.AsQueryable();
			if (where != null)
			{
				entities = entities.Where(where);
			}

			output.Total = entities.Count();

			if (orderyBy == null)
			{
				if (input.SortByDesc)
				{
					entities = entities.OrderByDescending(e => e.Id).Skip((input.Page.Value - 1) * input.Size.Value).Take(input.Size.Value);
				}
				else
				{
					entities = entities.Skip((input.Page.Value - 1) * input.Size.Value).Take(input.Size.Value);
				}
			}
			else
			{
				if (input.SortByDesc)
				{
					entities = entities.OrderByDescending(orderyBy).Skip((input.Page.Value - 1) * input.Size.Value).Take(input.Size.Value);
				}
				else
				{
					entities = entities.OrderBy(orderyBy).Skip((input.Page.Value - 1) * input.Size.Value).Take(input.Size.Value);
				}
			}

			output.Page = input.Page.Value;
			output.Size = input.Size.Value;
			output.Result = entities.ToList();
			return output;
		}
	}
}
