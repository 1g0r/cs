using System;
using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scraper.Parsers
{
	public class FindParser
	{
		private readonly IParsersRepository _parsersRepository;

		public FindParser(IParsersRepository parsersRepository)
		{
			_parsersRepository = parsersRepository;
		}

		public Parser Find(long id)
		{
			return _parsersRepository.Find(id)
				.ThrowIfNull(() => new EntityNotFoundException($"Parser with Id={id} not found."));
		}

		public Tuple<Parser, Source> Find(NormalizedUrl pageUrl, string tag = null)
		{
			if(pageUrl == null)
				throw new ArgumentNullException(nameof(pageUrl));

			if (string.IsNullOrWhiteSpace(tag))
				tag = null;

			var found = _parsersRepository.Find(pageUrl, tag).EnsureNotNull();
			
			// select result with longest match
			return found
				.OrderByDescending(x => x.Item1.Host.Host.Length)
				.Where(x => x.Item1.Path == null || pageUrl.LocalPath.StartsWith(x.Item1.Path))
				.OrderByDescending(x => x.Item1.Path?.Length ?? 0)
				.FirstOrDefault()
				.ThrowIfNull(() => new EntityNotFoundException($"Parser for page with URL '{pageUrl}' not found.")); 
		}

		public IEnumerable<Parser> List(long sourceId)
		{
			return _parsersRepository.List(sourceId);
		}
	}
}