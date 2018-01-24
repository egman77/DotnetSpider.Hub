using DotnetSpider.Enterprise.Domain;
using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Application.Log.Dto
{
	public class PaginationQueryLogInput : PaginationQueryInput
	{
		[Required]
		public string Identity { get; set; }

		public string NodeId { get; set; }

		public string LogType { get; set; }
	}
}
