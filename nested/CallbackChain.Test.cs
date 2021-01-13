using System;
using System.Linq;
using System.Collections.Generic;

namespace LoL
{
    internal class DataAccesor : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Dispose Accessor");
        }

        public DataReader GetReader()
        {
            return new DataReader();
        }
    }

    internal class DataReader : IDisposable
    {
        private string[] _items = new [] { "First", "Second", "Third" };
        private int _currentIndex = 0;
        private bool _isDisposed;

        public void Dispose()
        {
            _isDisposed = true;
            Console.WriteLine("Dispose Reader");
        }

        public bool Read(out string item)
        {
            if (_isDisposed)
                throw new Exception("Reader is disposed Read can't be performed!");
            if (_currentIndex < _items.Length)
            {
                item = _items[_currentIndex++];
                return true;
            }
            item = "";
            return false;
        }
    }

    class Program
    {
        private static IEnumerable<string> ExecuteSafe(string signature, string error, Func<IEnumerable> callback)
        {
            try
            {
                Console.WriteLine(signature);
                return callback();
            }
            catch (Exception ex)
            {
                Console.WriteLine(error + ex.ToString());
                return Enumerable.Empty<string>();
            }
        }

        private static IEnumerable<string> CreateAccessor(Func<DataAccessor, IEnumerable<string>> callback)
        {
            using (var accessor = new DataAccessor())
            {
                foreach (var item in callback(accessor))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<string> CreateReader(Func<DataReader> factory, Func<DataReader, IEnumerable<string>> callback)
        {
            using (var reader = factory())
            {
                foreach (var item in callback(reader))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<string> ReadItems(DataReader reader)
        {
            string item;
            while (reader.Read(out item))
            {
                yield return item;
            }
        }

        private static IEnumerable<string> ReadItems(DataReader reader, Func<IEnumerable<string>, IEnumerable<string>> callback)
        {
            return callback(ReadItems(reader));
        }

        private static IEnumerable<string> Map(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                yield return $"\t-{value}";
            }
        }

        static void Main()
        {
            var text = CallbackChain<IEnumerable<string>>
                .Enter(ExecuteSafe, "Start main", "Error: ");
                .Then<DataAccessor>(CreateAccessor)
                .With(a => a.GetReader())
                .Then(CreateReader)
                .Then<IEnumerable<string>>(ReadItems)
                .Return(Map)

            foreach (var line in text)
            {
                Console.WriteLine(line);
            } 
            Console.ReadKey()               
        }
    }
}
