public static class FilePathHelper
	{
		public static byte[] ToBytes(this string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return null;
			var bytes = new byte[filePath.Length * sizeof (char)];
			Buffer.BlockCopy(filePath.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		public static string ToFilePath(this byte[] bytes)
		{
			if (bytes.IsNullOrEmpty())
				return null;
			var chars = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
	}
