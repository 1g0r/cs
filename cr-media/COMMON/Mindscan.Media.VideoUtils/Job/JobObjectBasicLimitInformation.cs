using System;
using System.Runtime.InteropServices;

namespace Mindscan.Media.VideoUtils.Job
{
	[StructLayout(LayoutKind.Sequential)]
	struct JobObjectBasicLimitInformation
	{
		public Int64 PerProcessUserTimeLimit;
		public Int64 PerJobUserTimeLimit;
		public UInt32 LimitFlags;
		public UIntPtr MinimumWorkingSetSize;
		public UIntPtr MaximumWorkingSetSize;
		public UInt32 ActiveProcessLimit;
		public UIntPtr Affinity;
		public UInt32 PriorityClass;
		public UInt32 SchedulingClass;
	}
}