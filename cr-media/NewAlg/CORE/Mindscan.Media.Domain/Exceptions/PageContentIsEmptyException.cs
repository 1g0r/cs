using System;
using Mindscan.Media.Domain.Entities;

namespace Mindscan.Media.Domain.Exceptions
{
	public class PageContentIsEmptyException : Exception
	{
		public PageContentIsEmptyException(NormalizedUrl pageUrl)
			:base ($"Content of the page '{pageUrl}' is empty.")
		{
			
		}

		public PageContentIsEmptyException(Uri pageUrl) 
			:this(NormalizedUrl.Build(pageUrl))
		{

		}
	}
}