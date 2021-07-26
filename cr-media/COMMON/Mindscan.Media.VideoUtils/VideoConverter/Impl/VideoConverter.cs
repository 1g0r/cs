using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Mindscan.Media.VideoUtils.VideoConverter.Impl
{
	internal class VideoConverter : Job.Job
	{
		private AutoResetEvent _resultReceived;
		private readonly object _locker = new object();
		private VideoConverterResult? _result;
		private readonly IVideoConverterConfig _config;

		public VideoConverter(IVideoConverterConfig config) : base(config.EncoderPath)
		{
			_resultReceived = new AutoResetEvent(false);
			_config = config;
		}

		protected override ProcessStartInfo GetStartInfo(string exePath)
		{
			BindEvents();
			var ps = new ProcessStartInfo
			{
				CreateNoWindow = true,
				FileName = exePath,
				WorkingDirectory = Path.GetDirectoryName(exePath) ?? "",

				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8

			};
			return ps;
		}

		private void BindEvents()
		{
			Exited += OnExited;
		}

		private void UnbindEvents()
		{
			Exited -= OnExited;
		}

		private VideoConverterResult Result
		{
			get
			{
				return _result ?? VideoConverterResult.Error;
			}
			set
			{
				if (_result != null)
					return;

				lock (_locker)
				{
					if (_result != null)
						return;

					_result = value;
				}

				if (!_disposed)
					_resultReceived?.Set();
			}
		}

		public VideoConverterResult Convert(string sourceVideoPath, string outputVideoPath, string encodingOptions, int timeoutMilliseconds)
		{
			if (_config.Debug)
			{
				return VideoConverterResult.Success;
			}
			base.Run(new[]
			{
				//TODO: Нужно использовать -loglevel fatal -y
				// Т.к. процесс пишет логи в stdErr вместо stdOut 
				// и если out файл существует его перезаписываем
				$"-i \"{sourceVideoPath}\" -loglevel fatal -y {encodingOptions} \"{outputVideoPath}\""
			});

			if (!_resultReceived.WaitOne(timeoutMilliseconds))
			{
				return VideoConverterResult.Timeout;
			}
			return Result;
		}

		public VideoConverterResult GetScreenshot(int secondsToScreenshot, string sourceVideoPath, string outputScreenshotPath, string options, int timeoutMilliseconds)
		{
			if (_config.Debug)
			{
				return VideoConverterResult.Success;
			}
			base.Run(new[]
			{
				//TODO: Нужно использовать -loglevel fatal -y
				// Т.к. процесс пишет логи в stdErr вместо stdOut 
				// и если out файл существует его перезаписываем
				$"-ss {TimeSpan.FromSeconds(secondsToScreenshot)} -i \"{sourceVideoPath}\" -loglevel fatal -y {options} \"{outputScreenshotPath}\""
			});

			if (!_resultReceived.WaitOne(timeoutMilliseconds))
			{
				return VideoConverterResult.Timeout;
			}
			return Result;
		}

		private void OnExited(VideoUtils.Job.Job job, int exitCode)
		{
			switch (exitCode)
			{
				case 0:
					Result = VideoConverterResult.Success;
					break;
				default:
					Result = VideoConverterResult.Error;
					break;
			}
		}

		private volatile bool _disposed;
		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;
			UnbindEvents();

			_resultReceived.Dispose();
			_resultReceived = null;
			base.Dispose(disposing);
		}
	}
}