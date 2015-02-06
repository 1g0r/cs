using System.Text;

namespace mWF.StreamReaders
{
	internal sealed class MetaTagFilter : IBufferFilter
	{
		private const string StartTag = "<META ";
		private const char EndTag = '>';

		private readonly char[] _buffer;
		private readonly int _length;
		private readonly StringBuilder _symbols = new StringBuilder();
		private int _readIndex;

		public MetaTagFilter(char[] buffer, int startIndex, int count)
		{
			_buffer = buffer;
			_readIndex = startIndex;
			_length = startIndex + count;
		}

		public bool Read()
		{
			if (_readIndex >= _length)
				return false;

			if (IsStartTag() && SkipToEnd())
			{
				if (_readIndex < _length)
				{
					Value = _buffer[_readIndex++];
					return true;
				}
				return false;
			}
			if (_readIndex < _length)
			{
				Value = _buffer[_readIndex++];
				return true;
			}
			return false;
		}

		private bool IsStartTag()
		{
			if (_buffer[_readIndex] == '<')
			{
				while (_readIndex < _length && IsMeta())
				{
					_symbols.Append(_buffer[_readIndex++]);
				}
				if (_symbols.Length < StartTag.Length && _readIndex < _length)
				{
					_readIndex -= _symbols.Length;
					_symbols.Clear();
					return false;
				}
				return true;
			}
			return false;
		}

		private bool IsMeta()
		{
			return _symbols.Length == 0 && char.ToUpperInvariant(_buffer[_readIndex]) == StartTag[0] ||
			       _symbols.Length == 1 && char.ToUpperInvariant(_buffer[_readIndex]) == StartTag[1] ||
			       _symbols.Length == 2 && char.ToUpperInvariant(_buffer[_readIndex]) == StartTag[2] ||
			       _symbols.Length == 3 && char.ToUpperInvariant(_buffer[_readIndex]) == StartTag[3] ||
			       _symbols.Length == 4 && char.ToUpperInvariant(_buffer[_readIndex]) == StartTag[4] ||
			       _symbols.Length == 5 && char.ToUpperInvariant(_buffer[_readIndex]) == StartTag[5];
		}

		private bool SkipToEnd()
		{
			while (_readIndex < _length && _buffer[_readIndex] != EndTag)
			{
				_symbols.Append(_buffer[_readIndex++]);
			}
			if (_readIndex < _length && _buffer[_readIndex] == EndTag)
			{
				_symbols.Clear();
				_readIndex++;
				return true;
			}
			return false;
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
			return _symbols.ToString();
		}

		public int Flush(char[] buffer, ref int index, ref int count)
		{
			var result = 0;
			while (count > 0 && _symbols.Length > 0)
			{
				buffer[index++] = _symbols[0];
				_symbols.Remove(0, 1);
				count--;
				result++;
			}
			return result;
		}
	}
}
