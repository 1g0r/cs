using System;

namespace Mindscan.Media.Utils.Logger
{
	public interface ILogger
	{
		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }
		void Debug(string message);
		void DebugFormat(string format, params object[] args);
		void Debug(string message, Exception exception);
		void Info(string message);
		void Info(string message, Exception exception);
		void Warn(string message);
		void WarnFormat(string format, params object[] args);
		void Warn(string message, Exception exception);
		void Error(string message);
		void Error(string message, Exception exception);
		void Fatal(string message);
		void Fatal(string message, Exception exception);
	}
}