﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace SSICPAS.Utils
{
    public static class ParallelizationUtils
    {
        public static Task CreateInterlockedTask(Func<Task> func, ref int guard)
        {
            if (Interlocked.CompareExchange(ref guard, 1, 0) != 0)
                return Task.CompletedTask;

            try
            {
                return Task.Run(func);
            }
            finally
            {
                Interlocked.Exchange(ref guard, 0);
            }
        }

        public static T CreateInterlocked<T>(Func<T> func, ref int guard)
        {
            if (Interlocked.CompareExchange(ref guard, 1, 0) != 0)
                return default(T);

            try
            {
                return func.Invoke();
            }
            finally
            {
                Interlocked.Exchange(ref guard, 0);
            }
        }
    }
}
