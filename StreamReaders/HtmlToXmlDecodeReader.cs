using System.IO;
using System.Text;
using mWF.Helpers;

namespace mWF.StreamReaders
{
	internal class HtmlToXmlDecodeReader : StreamReader
	{
		private MnemonicDecoder _filter;
		
		public HtmlToXmlDecodeReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
			: base(path, encoding, detectEncodingFromByteOrderMarks)
		{ }

		public HtmlToXmlDecodeReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
			: base(stream, encoding, detectEncodingFromByteOrderMarks)
		{ }

		public override int Read(char[] buffer, int index, int count)
		{
			int readIndex = index;
			var flushedCount = 0;
			if (_filter != null)
			{
				flushedCount = _filter.Flush(buffer, ref index, ref count);
			}

			var readCount = base.Read(buffer, index, count);
			if (readCount == 0)
				return flushedCount;

			_filter = new MnemonicDecoder(new MetaTagFilter(buffer, readIndex, readCount + flushedCount));
			while (_filter.Read())
			{
				buffer[readIndex++] = _filter.Value;
			}

			return _filter.Count;
		}

		public override string ReadLine()
		{
			return base.ReadLine().Decode();
		}

		public override string ReadToEnd()
		{
			return base.ReadToEnd().Decode();
		}
	}
}
