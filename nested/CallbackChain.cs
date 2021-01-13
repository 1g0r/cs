using System;

namespace LoL
{
    public static class CallbackChain<TResult>
    {
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
        }
		
	public class NextCall<T1>
	{
	    private readonly Func<Func<T1, TResult>, TResult> _prev;
			
	    public NextCall(Func<Func<T1, TResult>, TResult> prev)
	    {
	        _prev = prev;
	    }
			
	    public NextCall<T2> Then<T2>(Func<T1, Func<T2, TResult>, TResult> next)
	    {
		return new NextCall<T2>(callback => _prev(x => next(x, callback)));
	    }

            public NextCallWith<T2> With<T2>(Func<T1, T2> factory)
            {
                return new NextCallWith<T2>(callback => _prev(x => callback(() => factory(x))));
            }

	    public TResult Return(Func<T1, TResult> next)
	    {
		return _prev(next);
	    }
	}
        
        public class NextCallWith<T>
        {
            private readonly Func<Func<Func<T>, TResult>, TResult> _prev;

            public NextCallWith(Func<Func<Func<T>, TResult>, TResult> prev)
            {
                _prev = prev;
            }

            public NextCall<T> Then(Func<Func<T>, Func<T, TResult>, TResult> next)
            {
                return new NextCall<T>(callback => _prev(x => next(x, callback)));
            }
        }

        public static NextCall Enter<T1, T2, T3>(Func<T1, T2, T3, Func<TResult>, TResult> prev, T1 a, T2 b, T3 c)
        {
            return new NextCall(callback => prev(a, b, c, callback));
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
		private static TResult SecureEval<TResult>(string prevMsg, string errorMsg, Func<TResult> executor)
		{
			try
			{
				Console.WriteLine(prevMsg);
				var result = executor();
				Console.WriteLine("Exit SecureEval");
				return result;
			}
			catch(Exception ex)
			{
				Console.WriteLine(errorMsg + ex.ToString());
				throw;
			}
		}
		
		private static TResult OpenConnection<TResult>(Func<SqlConn, TResult> cmdExecutor)
		{
			using(var conn = new SqlConn())
			{
				conn.Open();
				return cmdExecutor(conn);
			}
		}
		
		private static TResult OpenReader<TResult>(SqlConn conn, Func<SqlReader, TResult> resultReader)
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
			Console.WriteLine("Result = {0}, Type = {1}", t, t.GetType());
			
			Console.WriteLine("");
			
			var tt = CallbackChain<int>
				.Run("Begin secure", "fooo", SecureEval)
				.Then<SqlConn>(OpenConnection)
				.Then<SqlReader>(OpenReader)
				.Then(ReadInt);
			Console.WriteLine("Result = {0}, Type = {1}", tt, tt.GetType());
        }
    }
}
