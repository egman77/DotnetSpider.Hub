using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Models.AccountViewModels
{
	public class LoginViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[MinLength(6)]
		[MaxLength(18)]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}
}
