using System;
using System.Linq;
using Mindscan.Media.Utils.Logger;
using Quartz.Logging;

namespace Mindscan.Media.Utils.Scheduler.Impl
{
	internal class QuartzLogProvider : ILogProvider
	{
		public Quartz.Logging.Logger GetLogger(string name)
		{
			name = name.Split('.').Last();
			return (level, func, exception, parameters) =>
			{
				if (func != null)
				{
					switch (level)
					{
						case LogLevel.Debug:
							LoggerManager.CreateLogger(name).Debug(func());
							break;
						case LogLevel.Error:
							LoggerManager.CreateLogger(name).Error(func());
							break;
						case LogLevel.Fatal:
							LoggerManager.CreateLogger(name).Fatal(func());
							break;
						case LogLevel.Trace:
						case LogLevel.Info:
							LoggerManager.CreateLogger(name).Info(func());
							break;
						case LogLevel.Warn:
							LoggerManager.CreateLogger(name).Warn(func());
							break;
					}
				}

				return true;
			};
		}

		public IDisposable OpenNestedContext(string message)
		{
			throw new NotImplementedException();
		}

		public IDisposable OpenMappedContext(string key, string value)
		{
			throw new NotImplementedException();
		}
	}
}
