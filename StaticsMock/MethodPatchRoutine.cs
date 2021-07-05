using System;
using System.Reflection;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;

namespace LoL
{
    /// <summary>
    /// Uses library https://github.com/MonoMod/MonoMod/blob/master/README-RuntimeDetour.md
    /// </summary>
    public class MethodPatchRoutine : IDisposable
    {
        private static readonly object _locker = new object();
        private static readonly HashSet<MethodBase> _wrapped = new HashSet<MethodBase>();
        private readonly List<Detour> _detours = new List<Detour>();

        public void Dispose()
        {
            Free();
        }

        public void Free()
        {
            lock(_locker)
            {
                _detours.ForEach(x => {
                    x.Dispose();
                    _wrapped.Remove(x.Method);
                });
                _detours.Clear();
            }
        }

        public void Static(Action target, Action deputy)
        {
            SwapMethod(
                IsStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Static<TResult>(Func<TResult> target, Func<TResult> deputy)
        {
            SwapMethod(
                IsStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Static<T, TResult>(Func<T, TResult> target, Func<T, TResult> deputy)
        {
            SwapMethod(
                IsStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Static<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> target, Func<T1, T2, T3, TResult> deputy)
        {
            SwapMethod(
                IsStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Instance<TClass>(Action target, Action<TClass> deputy)
            where TClass : class
        {
            SwapMethod(
                NotStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Instance<TClass, TResult>(Func<TResult> target, Func<TClass, TResult> deputy)
            where TClass : class
        {
            SwapMethod(
                NotStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Instance<TClass, T, TResult>(Func<T, TResult> target, Func<TClass, T, TResult> deputy)
            where TClass : class
        {
            SwapMethod(
                NotStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Instance<TClass, T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> target, Func<TClass, T1, T2, T3, TResult> deputy)
            where TClass : class
        {
            SwapMethod(
                NotStatic(NotNull(target?.GetMethodInfo(), nameof(target))),
                IsStatic(NotNull(deputy?.GetMethodInfo(), nameof(deputy)))
            );
        }

        public void Ctor<T>(Action<T> deputy)
            where T : class
        {
            SwapMethod(
                NotAbstract<T>().GetConstructor(Type.EmptyTypes),
                IsStatic(NotNull(deputy.GetMethodInfo(), nameof(deputy)))
            );
        }

        private MethodBase NotNull(MethodBase method, string name)
        {
            if (method == null)
                throw new ArgumentNullException(name ?? nameof(method));
            return method;
        }

        private static MethodBase IsStatic(MethodBase method)
        {
            if (!method.IsStatic)
                throw new ArgumentException($"Method {method} should be static.");

            return method;
        }

        private static MethodBase NotStatic(MethodBase method)
        {
            if (method.IsStatic)
                throw new ArgumentException($"Method {method} should not be static.");

            return method;
        }

        private static Type NotAbstract<T>() where T : class
        {
            var result = typeof(T);

            if (result.IsAbstract)
                throw new ArgumentException($"Type {result} can not be abstract");

            return result;
        }
        
        private void SwapMethod(MethodBase target, MethodBase deputy)
        {
            lock(_locker)
            {
                if (_wrapped.Add(target))
                {
                    _detours.Add(new Detour(target, deputy));
                }
                else
                {
                    throw new InvalidOperationException($"Method {target} already has a deputy");
                }
            }
        }
    }
}
