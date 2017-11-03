using DotnetSpider.Enterprise.EntityFrameworkCore;

namespace DotnetSpider.Enterprise.Application.Log
{
	public class ApplicationLogAppService : AppServiceBase, IApplicationLogAppService
	{
		public ApplicationLogAppService(ApplicationDbContext dbcontext)
			: base(dbcontext)
		{

		}

		public void Log(Domain.Entities.Logs.Exception ex)
		{
			DbContext.Exceptions.Add(ex);
			DbContext.SaveChanges();
		}
	}
}
