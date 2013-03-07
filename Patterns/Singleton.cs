using System;

// http://csharpindepth.com/articles/general/singleton.aspx for more indepth explanation
namespace Patterns
{
    /// <summary>
    /// 8th try
    /// Without locks, fully lazy
    /// </summary>
    public class Singleton
    {
        private static readonly Lazy<Singleton> instance = new Lazy<Singleton>(() => new Singleton(), true);

        private Singleton() { }

        public static Singleton Instance()
        {
            return instance.Value;
        }

        public void DoSomething() { } // must be threadsafe
    }

    /// <summary>
    /// 7th try
    /// Without locks, fully lazy
    /// </summary>
    public class SimpleLazySingleton
    {
        private static class SimpleLazySingletonNested
        {
            // only one thread in ever in the static initialiser guaranteed by the framework
            // can acess the parent classes private construtor
            internal static readonly SimpleLazySingleton instance = new SimpleLazySingleton();
            static SimpleLazySingletonNested() { } // will only initialise on first use
        }

        private SimpleLazySingleton() { }

        public static void DoSomethingStatic() { }

        public static SimpleLazySingleton Instance()
        {
            return SimpleLazySingletonNested.instance;
        }

        public void DoSomething() { } // must be threadsafe
    }

    /// <summary>
    /// 6th try 
    /// Without locks, Not very lazy
    /// </summary>
    public class SimpleNotVeryLazySingleton
    {
        // only one thread in ever in the static initialiser guaranteed by the framework
        private static readonly SimpleNotVeryLazySingleton instance = new SimpleNotVeryLazySingleton();

        static SimpleNotVeryLazySingleton(){} // will only initialise on first use, but thats not fully lazy see below

        private SimpleNotVeryLazySingleton(){}

        public static void DoSomethingStatic(){} // if this is called before Instance, instance is still instanciated

        public static SimpleNotVeryLazySingleton Instance()
        {
            return instance;
        }

        public void DoSomething(){} // must be threadsafe
    }

    /// <summary>
    /// 5th try 
    /// Without locks, Not lazy
    /// </summary>
    public class SimpleSingleton
    {
        // only one thread in ever in the static initialiser guaranteed by the framework
        private static readonly SimpleSingleton instance = new SimpleSingleton();

        private SimpleSingleton(){}

        public static SimpleSingleton Instance()
        {
            return instance;
        }

        public void DoSomething(){} // must be threadsafe
    }
    
    /// <summary>
    /// 4th try
    /// yup should work
    /// </summary>
    public class ThreadsafeWithDoubleCheckLockSingleton
    {
        private static readonly object mutex = new object();
        private static volatile ThreadsafeWithDoubleCheckLockSingleton instance;

        private ThreadsafeWithDoubleCheckLockSingleton()
        { }

        public static ThreadsafeWithDoubleCheckLockSingleton Instance()
        {
            if (instance == null) // in the beginning 2 threads might hit this 
            {
                lock (mutex)
                {
                    if (instance == null) // unless instance is virtual, this may cause problems
                    {
                        instance = new ThreadsafeWithDoubleCheckLockSingleton();
                    }
                }
            }
            return instance;
        }

        public void DoSomething()
        { } // must be threadsafe
    }

    /// <summary>
    /// 3rd Try
    /// Since instance is not marked volatile, funky race conditions may occur, sometime, maybe
    /// </summary>
    public class NotThreadsafeWithDoubleCheckLockSingleton
    {
        private static readonly object mutex = new object();
        private static NotThreadsafeWithDoubleCheckLockSingleton instance;

        private NotThreadsafeWithDoubleCheckLockSingleton()
        { }

        public static NotThreadsafeWithDoubleCheckLockSingleton Instance()
        {
            if (instance == null) // in the beginning 2 threads might hit this 
            {
                lock (mutex)
                {
                    if (instance == null) // unless instance is virtual, this may cause problems
                    {
                        instance = new NotThreadsafeWithDoubleCheckLockSingleton();
                    }
                }
            }
            return instance;
        }

        public void DoSomething()
        { } // must be threadsafe
    }

    /// <summary>
    /// 2nd Try
    /// The drawback here is that a lock is acquired every time Instance is called
    /// </summary>
    public class ThreadsafeWithLockAlwaysSingleton
    {
        private static readonly object mutex = new object();
        private static ThreadsafeWithLockAlwaysSingleton instance;

        private ThreadsafeWithLockAlwaysSingleton()
        {}

        public static ThreadsafeWithLockAlwaysSingleton Instance()
        {
            lock (mutex)
            {
                if (instance == null)
                {
                    instance = new ThreadsafeWithLockAlwaysSingleton();
                }
                return instance;    
            }
        }

        public void DoSomething()
        {} // must be threadsafe
    }
    
    /// <summary>
    /// 1st Try
    /// </summary>
    public class NotThreadsafeSingleton
    {
        private static NotThreadsafeSingleton instance;

        private NotThreadsafeSingleton()
        {}

        public static NotThreadsafeSingleton Instance()
        {
            if (instance == null) // not threadsafe
            {
                instance = new NotThreadsafeSingleton();
            }
            return instance;
        }

        public void DoSomething()
        {} // must be threadsafe
    }
}
