using System;

namespace LoL
{
     public static class Nested
    {
        public delegate TResult Wrap<T, TResult>(Func<T, TResult> callback);
        public delegate TResult Wrap<T1, T2, TResult>(T1 x, Func<T2, TResult> callback);
        
        public static Wrap<T, TResult> Start<T, TResult>(this Wrap<T, TResult> prev)
        {
            return new Wrap<T, TResult>(callback => prev(callback));
        }
        
        public static Wrap<T2, TResult> Start<T1, T2, TResult>(T1 x, Wrap<T1, T2, TResult> prev)
        {
            return new Wrap<T2, TResult>(callback => prev(x, callback));
        }
        
        public static Wrap<T2, TResult> Next<T1, T2, TResult>(this Wrap<T1, TResult> prev, Wrap<T1, T2, TResult> next)
        {
            return new Wrap<T2, TResult>(callback => prev(x => next(x, callback)));
        }
        
        public static Func<TResult> Last<T, TResult>(this Wrap<T, TResult> prev, Func<T, TResult> callback)
        {
            return () => prev(callback);
        }
    }
    
    public static class Program
    {
        private static string GetBool(Func<bool, string> callback)
        {
            var flag = false;
            return callback(flag);
        }
        
        private static string PassBool(bool flag, Func<bool, string> callback)
        {
            return callback(flag);
        }

        private static string GetInt(bool flag, Func<int, string> callback)
        {
            if (flag){
                return callback(0);
            }
            return callback(666);
        }
        
        private static string GetString(int value)
        {
            return value.ToString() + " Fuck!";
        }
        
        public static void Main(string[] args)
        {
            //var t = GetBool(b => GetInt(b, GetString));
            var gb = Nested.Start<bool, string>(GetBool);            
            var gi = gb.Next<bool, int, string>(GetInt);            
            var gs = gi.Last(GetString);
            Console.WriteLine(gs());
            
            var tt = Nested.Start<bool, bool, string>(true, PassBool)
                .Next<bool, int, string>(GetInt)
                .Last(GetString);
            Console.WriteLine(tt());
        }
    }
}
