using System;
using Mindscan.Media.Utils.Logger;
using Topshelf.Logging;

namespace Mindscan.Media.Utils.Host.Impl
{
	internal sealed class HostLogWriter: LogWriter
	{
		private readonly ILogger _logger;
		public HostLogWriter(ILogger logger)
		{
			_logger = logger;
		}
		public void Log(LoggingLevel level, object obj)
		{
			if (LoggingLevel.None == level)
			{
				return;
			}
			if (LoggingLevel.Debug == level)
			{
				Debug(obj);
			}
			if (LoggingLevel.Info == level)
			{
				Info(obj);
			}
			if (LoggingLevel.Warn == level)
			{
				Warn(obj);
			}
			if (LoggingLevel.Error == level)
			{
				Error(obj);
			}
			if (LoggingLevel.Fatal == level)
			{
				Fatal(obj);
			}
			if (LoggingLevel.All == level)
			{
				Debug(obj);
			}
		}

		public void Log(LoggingLevel level, object obj, Exception exception)
		{
			if (LoggingLevel.None == level)
			{
				return;
			}
			if (LoggingLevel.Debug == level)
			{
				Debug(obj, exception);
			}
			if (LoggingLevel.Info == level)
			{
				Info(obj, exception);
			}
			if (LoggingLevel.Warn == level)
			{
				Warn(obj, exception);
			}
			if (LoggingLevel.Error == level)
			{
				Error(obj, exception);
			}
			if (LoggingLevel.Fatal == level)
			{
				Fatal(obj, exception);
			}
			if (LoggingLevel.All == level)
			{
				Debug(obj, exception);
			}
		}

		public void Log(LoggingLevel level, LogWriterOutputProvider messageProvider)
		{
			if (LoggingLevel.None == level)
			{
				return;
			}
			if (LoggingLevel.Debug == level)
			{
				Debug(messageProvider);
			}
			if (LoggingLevel.Info == level)
			{
				Info(messageProvider);
			}
			if (LoggingLevel.Warn == level)
			{
				Warn(messageProvider);
			}
			if (LoggingLevel.Error == level)
			{
				Error(messageProvider);
			}
			if (LoggingLevel.Fatal == level)
			{
				Fatal(messageProvider);
			}
			if (LoggingLevel.All == level)
			{
				Debug(messageProvider);
			}
		}

		public void LogFormat(LoggingLevel level, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (LoggingLevel.None == level)
			{
				return;
			}
			if (LoggingLevel.Debug == level)
			{
				DebugFormat(formatProvider, format, args);
			}
			if (LoggingLevel.Info == level)
			{
				InfoFormat(formatProvider, format, args);
			}
			if (LoggingLevel.Warn == level)
			{
				WarnFormat(formatProvider, format, args);
			}
			if (LoggingLevel.Error == level)
			{
				ErrorFormat(formatProvider, format, args);
			}
			if (LoggingLevel.Fatal == level)
			{
				FatalFormat(formatProvider, format, args);
			}
			if (LoggingLevel.All == level)
			{
				DebugFormat(formatProvider, format, args);
			}
		}

		public void LogFormat(LoggingLevel level, string format, params object[] args)
		{
			if (LoggingLevel.None == level)
			{
				return;
			}
			if (LoggingLevel.Debug == level)
			{
				DebugFormat(format, args);
			}
			if (LoggingLevel.Info == level)
			{
				InfoFormat(format, args);
			}
			if (LoggingLevel.Warn == level)
			{
				WarnFormat(format, args);
			}
			if (LoggingLevel.Error == level)
			{
				ErrorFormat(format, args);
			}
			if (LoggingLevel.Fatal == level)
			{
				FatalFormat(format, args);
			}
			if (LoggingLevel.All == level)
			{
				DebugFormat(format, args);
			}
		}

		public void Debug(object obj)
		{
			_logger.Debug(obj?.ToString());
		}

		public void Debug(object obj, Exception exception)
		{
			_logger.Debug(obj?.ToString(), exception);
		}

		public void Debug(LogWriterOutputProvider messageProvider)
		{
			if(messageProvider != null)
				_logger.Debug(messageProvider()?.ToString());
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			_logger.Debug(string.Format(formatProvider, format, args));
		}

		public void DebugFormat(string format, params object[] args)
		{
			_logger.Debug(string.Format(format, args));
		}

		public void Info(object obj)
		{
			_logger.Info(obj?.ToString());
		}

		public void Info(object obj, Exception exception)
		{
			_logger.Info(obj?.ToString(), exception);
		}

		public void Info(LogWriterOutputProvider messageProvider)
		{
			if (messageProvider != null)
				_logger.Info(messageProvider()?.ToString());
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			_logger.Info(string.Format(formatProvider, format, args));
		}

		public void InfoFormat(string format, params object[] args)
		{
			_logger.Info(string.Format(format, args));
		}

		public void Warn(object obj)
		{
			_logger.Warn(obj?.ToString());
		}

		public void Warn(object obj, Exception exception)
		{
			_logger.Warn(obj?.ToString(), exception);
		}

		public void Warn(LogWriterOutputProvider messageProvider)
		{
			if (messageProvider != null)
				_logger.Warn(messageProvider()?.ToString());
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			_logger.Warn(string.Format(formatProvider, format, args));
		}

		public void WarnFormat(string format, params object[] args)
		{
			_logger.Warn(string.Format(format, args));
		}

		public void Error(object obj)
		{
			_logger.Error(obj?.ToString());
		}

		public void Error(object obj, Exception exception)
		{
			_logger.Error(obj?.ToString(), exception);
		}

		public void Error(LogWriterOutputProvider messageProvider)
		{
			if (messageProvider != null)
				_logger.Error(messageProvider()?.ToString());
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			_logger.Error(string.Format(formatProvider, format, args));
		}

		public void ErrorFormat(string format, params object[] args)
		{
			_logger.Error(string.Format(format, args));
		}

		public void Fatal(object obj)
		{
			_logger.Fatal(obj?.ToString());
		}

		public void Fatal(object obj, Exception exception)
		{
			_logger.Fatal(obj?.ToString(), exception);
		}

		public void Fatal(LogWriterOutputProvider messageProvider)
		{
			if (messageProvider != null)
				_logger.Fatal(messageProvider()?.ToString());
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			_logger.Fatal(string.Format(formatProvider, format, args));
		}

		public void FatalFormat(string format, params object[] args)
		{
			_logger.Fatal(string.Format(format, args));
		}

		public bool IsDebugEnabled => _logger.IsDebugEnabled;
		public bool IsInfoEnabled => _logger.IsInfoEnabled;
		public bool IsWarnEnabled => _logger.IsWarnEnabled;
		public bool IsErrorEnabled => _logger.IsErrorEnabled;
		public bool IsFatalEnabled => _logger.IsFatalEnabled;
	}
}