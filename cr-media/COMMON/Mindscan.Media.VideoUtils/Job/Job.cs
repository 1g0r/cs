using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mindscan.Media.VideoUtils.Job
{
	internal delegate void OnError(Job job, string errorData);
	internal delegate void OnExited(Job job, int exitCode);
	internal delegate void OnOutput(Job job, string output);
	internal abstract class Job : IDisposable
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		static extern IntPtr CreateJobObject(IntPtr a, string lpName);

		[DllImport("kernel32.dll")]
		static extern bool SetInformationJobObject(
			IntPtr hJob,
			JobObjectInfoType infoType,
			IntPtr lpJobObjectInfo,
			UInt32 cbJobObjectInfoLength);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AssignProcessToJobObject(
			IntPtr job,
			IntPtr process);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);

		private IntPtr _handle;
		private readonly Process _process;
		private bool _disposed;

		protected Job(string exePath)
		{
			_handle = CreateJobObject(IntPtr.Zero, null);
			var info = new JobObjectBasicLimitInformation
			{
				LimitFlags = 0x2000
			};

			var extendedInfo = new JobObjectExtendedLimitInformation
			{
				BasicLimitInformation = info
			};

			var infoType = typeof(JobObjectExtendedLimitInformation);
			var length = Marshal.SizeOf(infoType);
			var extendedInfoPtr = Marshal.AllocHGlobal(length);
			Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

			var setResult = SetInformationJobObject(
				_handle,
				JobObjectInfoType.ExtendedLimitInformation,
				extendedInfoPtr,
				(uint)length);

			if (setResult)
			{
				_process = new Process();
				ConfigureProcess(exePath);
				return;
			}

			var lastError = Marshal.GetLastWin32Error();
			var message = "Unable to set information. Error: " + lastError;
			throw new Exception(message);
		}

		~Job()
		{
			Dispose(false);
		}

		private void ConfigureProcess(string exePath)
		{
			_process.StartInfo = GetStartInfo(exePath);
			_process.EnableRaisingEvents = true;
			_process.Exited += JobOnExited;
			_process.ErrorDataReceived += JobOnError;
			_process.OutputDataReceived += JobOnOutput;
		}

		private bool AddProcess(IntPtr processHandle)
		{
			return AssignProcessToJobObject(_handle, processHandle);
		}

		protected abstract ProcessStartInfo GetStartInfo(string exePath);

		private void JobOnOutput(object sender, DataReceivedEventArgs dataReceivedEventArgs)
		{
			if (_disposed)
				return;

			string str = dataReceivedEventArgs.Data;
			if (!string.IsNullOrEmpty(str))
			{
				CallEvent(StdOut, output =>
				{
					output.Invoke(this, str);
				});
			}
		}

		private void JobOnError(object sender, DataReceivedEventArgs dataReceivedEventArgs)
		{
			if (_disposed)
				return;

			string errorData = dataReceivedEventArgs.Data;
			if (!string.IsNullOrEmpty(errorData))
			{
				CallEvent(Error, error =>
				{
					error.Invoke(this, errorData);
				});
			}
		}

		private void JobOnExited(object sender, EventArgs eventArgs)
		{
			if (_disposed)
				return;

			if (_process.HasExited)
			{
				CallEvent(Exited, exited =>
				{
					exited.Invoke(this, _process.ExitCode);
				});
			}
		}

		private void CallEvent<T>(T @event, Action<T> invoker) where T : class
		{
			var handler = @event;
			if (handler != null)
			{
				try
				{
					invoker.Invoke(handler);
				}
				catch (Exception e)
				{
					throw new EventHandleException("Unable to handle event", e);
				}
			}
		}


		#region Dispose
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
				GC.SuppressFinalize(this);

			_process.Exited -= JobOnExited;
			_process.ErrorDataReceived -= JobOnError;
			_process.OutputDataReceived -= JobOnOutput;
			Exited = null;
			Error = null;
			_process.Dispose();
			CloseHandle(_handle);
			_handle = IntPtr.Zero;

			_disposed = true;
		}

		#endregion

		#region Events
		public event OnExited Exited;
		public event OnError Error;
		public event OnOutput StdOut;
		#endregion

		#region Properties

		public bool HasExited => _process.HasExited;

		public int ExitCode => _process.ExitCode;

		#endregion

		protected virtual void Run(string[] args)
		{
			_process.StartInfo.Arguments += " " + string.Join(" ", args);
			if (_process.Start() && AddProcess(_process.Handle))
			{
				_process.BeginOutputReadLine();
				_process.BeginErrorReadLine();
				return;
			}
			throw new Exception("Unable to start process!");
		}
	}
}
