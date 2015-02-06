namespace mWF.StreamReaders
{
	internal interface IBufferFilter
	{
		bool Read();

		char Value { get; }

		int Count { get; }

		string Flush();

		int Flush(char[] buffer, ref int index, ref int count);
	}
}
