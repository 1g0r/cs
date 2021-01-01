using System;

namespace Rextester
{
    public static class Program
    {
        private static string GetBool(Func<bool, string> callback)
        {
            var flag = false;
            return callback(flag);a
        }
        
        private static string PassBool(bool flag, Func<bool, string> callback){
            return callback(flag);
        }

        private static string GetInt(bool flag, Func<int, string> callback)
        {
            if (flag){
                return callback(0);
            }
            return callback(666);
        }

        private static string GetString(int value){
            return value.ToString() + " Fuck!";
        }
        
        private static Func<Func<T1, T>, T> Nested<T1, T>(this Func<Func<T1, T>, T> prev){
            return (Func<T1, T> callback) => prev(callback);
        }
        
        private static Func<Func<T2, T>, T> Next<T1, T2, T>(this Func<Func<T1, T>, T> prev, Func<T1, Func<T2, T>, T> next){
            return (Func<T2, T> callback) => prev((T1 x) => next(x, callback));
        }
        
        private static Func<T> Last<T1, T>(this Func<Func<T1, T>, T> prev, Func<T1, T> callback){
            return () => prev(callback);
        }
        
        public static void Main(string[] args)
        {
            //var t = GetBool(b => GetInt(b, GetString));
            var gb = Nested<bool, string>(GetBool);            
            var gi = gb.Next<bool, int, string>(GetInt);            
            var gs = gi.Last(GetString);
            Console.WriteLine(gs());
            
            var tt = Nested<bool, String>((f) => PassBool(true, f))
                .Next<bool, int, string>(GetInt)
                .Last(GetString);
            Console.WriteLine(tt());
        }
    }
}
