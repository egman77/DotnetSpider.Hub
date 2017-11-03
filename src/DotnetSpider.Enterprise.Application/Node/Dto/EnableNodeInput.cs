using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
    public class NodeEnable
	{
		[Required]
		public string Id { get; set; }
		public bool Enable { get; set; }
	}
}
