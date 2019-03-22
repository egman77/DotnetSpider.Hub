using System.Net.Http;
namespace DotnetSpider.Hub.Application.Scheduler.Dtos
{
	public class SchedulerJobDto
	{
		public virtual string Id { get; set; }

		public virtual string Name { get; set; }

		public virtual string Cron { get; set; }

		public virtual string Url { get; set; }

        /// <summary>
        /// 回调的方式
        /// 默认为get
        /// </summary>
        public virtual HttpMethod Method { get; set; } = HttpMethod.Get;

        //改名为内容
		//public virtual string Data { get; set; }
        public virtual string Content { get; set; }

        /// <summary>
        /// 组名?
        /// </summary>
        public virtual string Group { get; set; } = "DOTNETSPIDER.HUB";
	}
}
