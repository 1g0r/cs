using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scheduler.Sources;
using SourceFilter = Mindscan.Media.Domain.Entities.Scheduler.SourceFilter;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Impl
{
	[DebuggerStepThrough]
	internal class SourcesFacade: ISourcesFacade
	{
		private readonly Lazy<AddSource> _addSource;
		private readonly Lazy<FindSource> _findSource;
		private readonly Lazy<UpdateSource> _updateSource;
		private readonly Lazy<DeleteSource> _deleteSource;

		[DebuggerStepThrough]
		public SourcesFacade(ISourcesRepository sourcesRepository)
		{
			_addSource = new Lazy<AddSource>(() => new AddSource(sourcesRepository));
			_findSource = new Lazy<FindSource>(() => new FindSource(sourcesRepository));
			_updateSource = new Lazy<UpdateSource>(() => new UpdateSource(sourcesRepository));
			_deleteSource = new Lazy<DeleteSource>(() => new DeleteSource(sourcesRepository));
		}
		public Source Add(Source source)
		{
			return _addSource.Value.Add(source);
		}

		public Source Find(long id)
		{
			return _findSource.Value.Find(id);
		}

		public IEnumerable<Source> Find(SourceFilter filter)
		{
			return _findSource.Value.Find(filter);
		}

		public Source Update(Source source)
		{
			return _updateSource.Value.Update(source);
		}

		public int Delete(Source source)
		{
			return _deleteSource.Value.Delete(source);
		}
	}
}