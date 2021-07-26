using System.IO;
using System.Net;
using System.Text;

namespace Mindscan.Media.HtmlParser.Core.Helpers
{
	internal sealed class MnemonicDecoder : StreamReader
	{
		private const string StartTag = "&#";
		private const char EndTag = ';';
		private const int MaxLength = 12;
		private readonly StringBuilder _buffer = new StringBuilder();

		public MnemonicDecoder(Stream stream) : base(stream)
		{
		}

		public override int Read()
		{
			if (_buffer.Length > 0)
			{
				return Pop();
			}

			var code = base.Read();
			if (code < 0)
				return -1;

			Value = (char)code;
			if (IsStartTag())
			{
				_buffer.Append(Value);
				return ReadMnemonic();
			}
			return Value;
		}

		private bool IsStartTag()
		{
			return _buffer.Length == 0 && Value.Equals(StartTag[0]);
		}

		private int ReadMnemonic()
		{
			var endFound = false;
			while (!endFound && _buffer.Length > 0 && _buffer.Length < MaxLength)
			{
				var symbol = base.Read();
				if (symbol == -1)
				{
					break;

				}
				Value = (char)symbol;
				if (Value.Equals(EndTag))
				{
					endFound = true;
				}
				_buffer.Append(Value);
			}
			if (endFound)
			{
				Decode();
			}
			return Pop();
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

		private int Pop()
		{
			Value = _buffer[0];
			_buffer.Remove(0, 1);
			return Value;
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
			return _buffer.ToString();
		}

		public int Flush(char[] buffer, ref int index, ref int count)
		{
			var flashedCount = 0;
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