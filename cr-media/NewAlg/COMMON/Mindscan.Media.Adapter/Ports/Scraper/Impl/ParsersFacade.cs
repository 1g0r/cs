using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scraper.Parsers;

namespace Mindscan.Media.Adapter.Ports.Scraper.Impl
{
	internal class ParsersFacade : IParsersFacade
	{
		private readonly Lazy<AddParser> _addParser;
		private readonly Lazy<FindParser> _findParser;
		private readonly Lazy<FindParserJson> _findParserJson;
		private readonly Lazy<UpdateParser> _updateParser;
		private readonly Lazy<ValidateParserJson> _validateParserJson;
		private readonly Lazy<RunParser> _runner;
		private readonly Lazy<DeleteParser> _deleteParser;

		public ParsersFacade(
			ISourcesRepository sourcesRepository, 
			IParsersRepository parserRepository, 
			IParserProcessor processor)
		{
			_addParser = new Lazy<AddParser>(() => new AddParser(sourcesRepository, parserRepository));
			_findParser = new Lazy<FindParser>(() => new FindParser(parserRepository));
			_findParserJson = new Lazy<FindParserJson>(() => new FindParserJson(parserRepository));
			_validateParserJson = new Lazy<ValidateParserJson>(() => new ValidateParserJson(processor));
			_updateParser = new Lazy<UpdateParser>(() => new UpdateParser(parserRepository, _validateParserJson.Value));
			_runner = new Lazy<RunParser>(() => new RunParser(processor, _validateParserJson.Value));
			_deleteParser = new Lazy<DeleteParser>(() => new DeleteParser(parserRepository));


		}

		public Parser Find(long id)
		{
			return _findParser.Value.Find(id);
		}

		public Tuple<Parser, Source> Find(NormalizedUrl pageUrl, string tag = null)
		{
			return _findParser.Value.Find(pageUrl, tag);
		}

		public Parser Add(Parser parser)
		{
			return _addParser.Value.Add(parser);
		}

		public IEnumerable<Parser> List(long sourceId)
		{
			return _findParser.Value.List(sourceId);
		}

		public Parser Update(Parser parser)
		{
			return _updateParser.Value.Update(parser);
		}

		public Parser Update(Parser parser, string json)
		{
			return _updateParser.Value.Update(parser, json);
		}

		public string FindJson(long id)
		{
			return _findParserJson.Value.FindJson(id);
		}

		public string ValidateJson(string json)
		{
			return _validateParserJson.Value.Validate(json);
		}

		public string Run(string json, string content, NormalizedUrl pageUrl, bool debug)
		{
			return _runner.Value.Run(json, content, pageUrl, debug);
		}

		public int Delete(Parser parser)
		{
			return _deleteParser.Value.Delete(parser);
		}
	}
}