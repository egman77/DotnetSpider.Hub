using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Application.Log.Dto
{
	public class AddLogInput
	{
		[Required]
		public dynamic LogInfo { get; set; }

		[Required]
		public string Identity { get; set; }
	}
}
