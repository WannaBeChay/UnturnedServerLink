﻿using Nito.AsyncEx;
using SDG.Unturned;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace USLModule.Utils
{
    public static class AsyncHelper
    {
        /// <summary>
        ///     Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsync(this MethodInfo method)
        {
            return method.ReturnType.IsTaskOrTaskOfT();
        }

        public static bool IsTaskOrTaskOfT(this Type type)
        {
            return type == typeof(Task) || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>
        ///     Returns void if given type is Task.
        /// Return T, if given type is Task{T}.
        /// Returns given type otherwise.
        /// </summary>
        public static Type UnwrapTask(Type type)
        {
            if (type == typeof(Task))
            {
                return typeof(void);
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return type.GenericTypeArguments[0];
            }

            return type;
        }

        /// <summary>
        ///     Runs a async method synchronously.
        /// </summary>
        /// <param name="func">A function that returns a result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result of the async operation</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncContext.Run(func);
        }

        /// <summary>
        ///     Runs a async method synchronously.
        /// </summary>
        /// <param name="action">An async action</param>
        public static void RunSync(Func<Task> action)
        {
            AsyncContext.Run(action);
        }

        public static void Schedule(string name, Func<Task> task, int interval, Action<Exception> exceptionHandler = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        await task();
                        await Task.Delay(interval);
                    }
                }
                catch (Exception e)
                {
                    if (exceptionHandler != null)
                    {
                        exceptionHandler(e);
                    }
                    else
                    {
                        CommandWindow.Log($"{e} Exception occured in task \"{name}\"\n");
                    }
                }
            });
        }
    }
}
