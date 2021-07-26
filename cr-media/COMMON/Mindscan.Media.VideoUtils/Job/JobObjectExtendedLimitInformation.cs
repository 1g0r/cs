using System;
using System.Runtime.InteropServices;

namespace Mindscan.Media.VideoUtils.Job
{
	[StructLayout(LayoutKind.Sequential)]
	struct JobObjectExtendedLimitInformation
	{
		public JobObjectBasicLimitInformation BasicLimitInformation;
		public IoCounters IoInfo;
		public UIntPtr ProcessMemoryLimit;
		public UIntPtr JobMemoryLimit;
		public UIntPtr PeakProcessMemoryUsed;
		public UIntPtr PeakJobMemoryUsed;
	}
}