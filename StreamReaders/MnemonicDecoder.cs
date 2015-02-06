using System.Net;
using System.Text;
using mWF.Helpers;

namespace mWF.StreamReaders
{
	internal sealed class MnemonicDecoder : IBufferFilter
	{
		private const string StartTag = "&#";
		private const char EndTag = ';';
		private const int MaxLength = 12;
		private readonly IBufferFilter _filter;
		private readonly StringBuilder _buffer = new StringBuilder();

		public MnemonicDecoder(IBufferFilter filter)
		{
			_filter = filter;
		}

		public bool Read()
		{
			if (_buffer.Length > 0)
			{
				return Pop();
			}

			if (!_filter.Read())
				return false;

			if (IsStartTag())
			{
				return ReadMnemonic();
			}
			Value = _filter.Value;
			return true;
		}

		private bool IsStartTag()
		{
			if (_buffer.Length == 0 && _filter.Value.Equals(StartTag[0]))
			{
				_buffer.Append(_filter.Value);
				return true;
			}
			return false;
		}

		private bool ReadMnemonic()
		{
			var endFound = false;
			while (!endFound && _filter.Read() && _buffer.Length > 0 && _buffer.Length < MaxLength)
			{
				if (_buffer.Length == 1 && _filter.Value.Equals(StartTag[1]))
				{
					_buffer.Append(_filter.Value);
				}
				else if (_filter.Value.IsMnemonicName())
				{
					_buffer.Append(_filter.Value);
				}
				else if (_filter.Value.Equals(EndTag))
				{
					_buffer.Append(_filter.Value);
					endFound = true;
				}
			}
			if (endFound)
			{
				Decode();
				return Pop();
			}
			return false;
		}

		private void Decode()
		{
			var mem = _buffer.ToString();
			_buffer.Clear();
			if (mem.IsXmlMnemonic())
			{
				_buffer.Append(mem);
			}
			else
			{
				_buffer.Append(WebUtility.HtmlDecode(mem));
			}
		}

		private bool Pop()
		{
			Value = _buffer[0];
			_buffer.Remove(0, 1);
			return true;
		}

		private char _value;

		public char Value
		{
			get { return _value; }
			private set { _value = value; Count++; }
		}

		public int Count { get; private set; }
		public string Flush()
		{
			return _filter.Flush() + _buffer;
		}

		public int Flush(char[] buffer, ref int index, ref int count)
		{
			var flashedCount = _filter.Flush(buffer, ref index, ref count);
			while (count > 0 && _buffer.Length > 0)
			{
				buffer[index++] = _buffer[0];
				_buffer.Remove(0, 1);
				count--;
				flashedCount++;
			}
			return flashedCount;
		}
	}
}
