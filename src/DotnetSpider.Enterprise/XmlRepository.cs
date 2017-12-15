using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace DotnetSpider.Enterprise
{
	public class XmlRepository : IXmlRepository
	{
		private readonly string _filePath;
		private readonly ICommonConfiguration _config;

		public XmlRepository(ICommonConfiguration config)
		{
			_config = config;
		}

		public virtual IReadOnlyCollection<XElement> GetAllElements()
		{
			return GetAllElementsCore().ToList().AsReadOnly();
		}

		public virtual void StoreElement(XElement element, string friendlyName)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			element.Save(File.OpenWrite(_filePath));
		}

		private IEnumerable<XElement> GetAllElementsCore()
		{
			yield return XElement.Load(_filePath);
		}
	}
}
