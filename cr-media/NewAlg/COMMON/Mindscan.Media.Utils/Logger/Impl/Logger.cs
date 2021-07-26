using System;
using log4net;

namespace Mindscan.Media.Utils.Logger.Impl
{
	internal class Logger : ILogger
	{
		private readonly ILog _log;

		public Logger(string name)
		{
			_log = LogManager.GetLogger(name);
		}

		public bool IsDebugEnabled => _log.IsDebugEnabled;

		public bool IsInfoEnabled => _log.IsInfoEnabled;
		public bool IsWarnEnabled => _log.IsWarnEnabled;
		public bool IsErrorEnabled => _log.IsErrorEnabled;
		public bool IsFatalEnabled => _log.IsFatalEnabled;
		public void Debug(string message)
		{
			if (IsDebugEnabled)
			{
				_log.Debug(message);
			}
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				_log.DebugFormat(format, args);
			}
		}

		public void Debug(string message, Exception exception)
		{
			if (IsDebugEnabled)
			{
				_log.DebugFormat("{0} {1}", message, exception.ToString());
			}	
		}

		public void Info(string message)
		{
			if (IsInfoEnabled)
			{
				_log.Info(message);
			}
		}

		public void Info(string message, Exception exception)
		{
			if (IsInfoEnabled)
			{
				_log.InfoFormat("{0} {1}", message, exception.ToString());
			}
		}

		public void Warn(string message)
		{
			if (IsWarnEnabled)
			{
				_log.Warn(message);
			}
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				_log.WarnFormat(format, args);
			}
		}

		public void Warn(string message, Exception exception)
		{
			if (IsWarnEnabled)
			{
				_log.WarnFormat("{0} {1}", message, exception.ToString());
			}
		}

		public void Error(string message)
		{
			_log.Error(message);
		}

		public void Error(string message, Exception exception)
		{
			_log.ErrorFormat("{0} {1}", message, exception.ToString());
		}

		public void Fatal(string message)
		{
			_log.Fatal(message);
		}

		public void Fatal(string message, Exception exception)
		{
			_log.FatalFormat("{0} {1}", message, exception.ToString());
		}
	}
}