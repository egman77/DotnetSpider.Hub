using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Hub.Core.Entities
{
	public class Config : AuditedEntity
	{
		[Required]
		[StringLength(32)]
		public string Name { get; set; }

		[Required]
		public string Value { get; set; }
	}
}
