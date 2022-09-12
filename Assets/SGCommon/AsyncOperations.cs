using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

namespace SolarGames.FlatArmy
{
    public class AsyncOperations
    {
        public delegate void DCallbackResult<TResult>(TResult result);
        public delegate TResult DOperation<T, TResult>(T info);

        private static Queue<System.Action> mainThreadQueue = new Queue<System.Action>();
        private static Queue<System.Action> asyncThreadQueue = new Queue<System.Action>();

        private static Timer timer;

        public static void Init()
        {
            timer = new Timer(AsyncTimerCallback, null, 0, 50);
        }

        public static void Stop()
        {
            if (timer != null)
                timer.Dispose();
        }

        private static void AsyncTimerCallback(object state)
        {
            lock (asyncThreadQueue)
            {
                if (asyncThreadQueue.Count > 0)
                    asyncThreadQueue.Dequeue()();
            }
        }

        public static void Invoke<T, TResult>(T input, DOperation<T, TResult> operation, DCallbackResult<TResult> callback)
        {
            //TResult result = operation(input);
            //callback(result);
            System.Action d = () =>
            {
                TResult result = operation(input);
                lock (mainThreadQueue)
                    mainThreadQueue.Enqueue(() => callback(result));
            };
            lock (asyncThreadQueue)
                asyncThreadQueue.Enqueue(d);
        }

        public static void Invoke<TResult>(Func<TResult> operation, DCallbackResult<TResult> callback)
        {
            System.Action d = () =>
            {
                TResult result = operation();
                lock (mainThreadQueue)
                    mainThreadQueue.Enqueue(() => callback(result));
            };
            lock (asyncThreadQueue)
                asyncThreadQueue.Enqueue(d);
        }

        public static void Update()
        {
            lock (mainThreadQueue)
            {
                if (mainThreadQueue.Count > 0)
                    mainThreadQueue.Dequeue()();
            }
        }

    }
}