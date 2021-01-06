using System;

namespace LoL
{
	public static class CallbackChain<TResult>
	{
		public static NextCall<T> Run<T>(Func<Func<T, TResult>, TResult> prev)
		{
			return new NextCall<T>(callback => prev(callback));
		}
		
		public static NextCall Run<T1, T2>(T1 a, T2 b, Func<T1, T2, Func<TResult>, TResult> prev)
		{
			return new NextCall(callback => prev(a, b, callback));
		}
		
		public class NextCall
		{
			private readonly Func<Func<TResult>, TResult> _prev;
			public NextCall(Func<Func<TResult>, TResult> prev)
			{
				_prev = prev;
			}

			public NextCall<T> Then<T>(Func<Func<T, TResult>, TResult> next)
			{
				return new NextCall<T>(callback => _prev(() => next(callback)));
			}
			public Func<TResult> Then(Func<TResult> next)
			{
				return () => _prev(next);
			}
		}
		
		public class NextCall<T> : NextCall
		{
			private readonly Func<Func<T, TResult>, TResult> _prev;
			public NextCall(Func<Func<T, TResult>, TResult> prev) : base(null)
			{
				_prev = prev;
			}
			public NextCall<TT> Then<TT>(Func<T, Func<TT, TResult>, TResult> next)
			{
				return new NextCall<TT>(callback => _prev(x => next(x, callback)));
			}
			public Func<TResult> Then(Func<T, TResult> next)
			{
				return () => _prev(next);
			}
		}
	}
	
	// DUMMY
	public class SqlReader : IDisposable 
	{
		public void Dispose()
		{
			Console.WriteLine("Reader disposed!");
		}
		public string Read()
		{
			return "Hello from reader!!";
		}
	}
	public class SqlConn : IDisposable 
	{
		public void Dispose()
		{
			Console.WriteLine("Connection disposed!");
		}
		public void Open(){
			Console.WriteLine("Connection opened!");
		}
		public SqlReader GetReader()
		{
			Console.WriteLine("Reader created!");
			return new SqlReader();
		}
	}
	
    public static class Program
    {
		private static string SecureEval(string prevMsg, string errorMsg, Func<string> executor)
		{
			try{
				Console.WriteLine(prevMsg);
				return executor();
			}
			catch(Exception ex){
				Console.WriteLine(errorMsg);
				throw;
			}
		}
		private static string OpenConnection(Func<SqlConn, string> cmdExecutor)
		{
			using(var conn = new SqlConn())
			{
				conn.Open();
				return cmdExecutor(conn);
			}
		}
		
		private static string OpenReader(SqlConn conn, Func<SqlReader, string> resultReader)
		{
			using(var reader = conn.GetReader())
			{
				return resultReader(reader);
			}
		}
		
		private static string Read(SqlReader reader)
		{
			return reader.Read();
		}
		
		/* int */
		private static int SecureEval(string prevMsg, string errorMsg, Func<int> executor)
		{
			try{
				Console.WriteLine(prevMsg);
				return executor();
			}
			catch(Exception ex){
				Console.WriteLine(errorMsg);
				throw;
			}
		}
		private static int OpenConnection(Func<SqlConn, int> cmdExecutor)
		{
			using(var conn = new SqlConn())
			{
				conn.Open();
				return cmdExecutor(conn);
			}
		}
		
		private static int OpenReader(SqlConn conn, Func<SqlReader, int> resultReader)
		{
			using(var reader = conn.GetReader())
			{
				return resultReader(reader);
			}
		}
		
		private static int ReadInt(SqlReader reader)
		{
			return 666;
		}
        
        
        public static void Main(string[] args)
        {
			var t = CallbackChain<string>
				.Run("Begin secure", "fooo", SecureEval)
				.Then<SqlConn>(OpenConnection)
				.Then<SqlReader>(OpenReader)
				.Then(Read);
			Console.WriteLine(t());
			
			Console.WriteLine("");
			
			var tt = CallbackChain<int>
				.Run("Begin secure", "fooo", SecureEval)
				.Then<SqlConn>(OpenConnection)
				.Then<SqlReader>(OpenReader)
				.Then(ReadInt);
			Console.WriteLine(tt());
        }
    }
}
