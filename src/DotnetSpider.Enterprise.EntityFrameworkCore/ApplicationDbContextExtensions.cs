using DotnetSpider.Enterprise.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DotnetSpider.Enterprise.EntityFrameworkCore
{
	public static class ApplicationDbContextExtensions
	{
		public static PagingQueryOutputDto PageList<TEntity, TKey>(this DbSet<TEntity> dbSet, PagingQueryInputDto input,
			Expression<Func<TEntity, bool>> where = null,
			Expression<Func<TEntity, TKey>> orderyBy = null) where TEntity : Entity<long>
		{
			input.Validate();
			PagingQueryOutputDto output = new PagingQueryOutputDto();
			IQueryable<TEntity> entities = dbSet.AsQueryable();
			if (where != null)
			{
				entities = entities.Where(where);
			}

			output.Total = entities.Count();

			if (orderyBy == null)
			{
				if (input.IsSortByDesc())
				{
					entities = entities.OrderByDescending(e => e.Id).Skip((input.Page - 1) * input.Size).Take(input.Size);
				}
				else
				{
					entities = entities.Skip((input.Page - 1) * input.Size).Take(input.Size);
				}
			}
			else
			{
				if (input.IsSortByDesc())
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
