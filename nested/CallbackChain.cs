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
}
