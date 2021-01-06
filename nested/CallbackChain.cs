using System;

namespace LoL
{
	public class Nxt<TResult>
	{
		private readonly Func<Func<TResult>, TResult> _prev;
		public Nxt(Func<Func<TResult>, TResult> prev)
		{
			_prev = prev;
		}
		
		public Nxt<T, TResult> Then<T>(Func<Func<T, TResult>, TResult> next)
		{
			return new Nxt<T, TResult>(callback => _prev(() => next(callback)));
		}
		public Func<TResult> Then(Func<TResult> next)
		{
			return () => _prev(next);
		}
	}
	public class Nxt<T, TResult> : Nxt<TResult>
	{
		private readonly Func<Func<T, TResult>, TResult> _prev;
		public Nxt(Func<Func<T, TResult>, TResult> prev) : base(null)
		{
			_prev = prev;
		}
		public Nxt<TT, TResult> Then<TT>(Func<T, Func<TT, TResult>, TResult> next)
		{
			return new Nxt<TT, TResult>(callback => _prev(x => next(x, callback)));
		}
		public Func<TResult> Then(Func<T, TResult> next)
		{
			return () => _prev(next);
		}
	}
	
	public static class Chain<TResult>
	{
		public static Nxt<T, TResult> Run<T>(Func<Func<T, TResult>, TResult> prev)
		{
			return new Nxt<T, TResult>(callback => prev(callback));
		}
		
		public static Nxt<TResult> Run<T1, T2>(T1 a, T2 b, Func<T1, T2, Func<TResult>, TResult> prev)
		{
			return new Nxt<TResult>(callback => prev(a, b, callback));
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
			var t = Chain<string>
				.Run("Begin secure", "fooo", SecureEval)
				.Then<SqlConn>(OpenConnection)
				.Then<SqlReader>(OpenReader)
				.Then(Read);
			Console.WriteLine(t());
			
			var tt = Chain<int>
				.Run("Begin secure", "fooo", SecureEval)
				.Then<SqlConn>(OpenConnection)
				.Then<SqlReader>(OpenReader)
				.Then(ReadInt);
			Console.WriteLine(tt());
        }
    }
}
