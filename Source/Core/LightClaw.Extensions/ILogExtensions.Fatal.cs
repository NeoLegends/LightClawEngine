using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Core;

namespace LightClaw.Extensions
{
    public static partial class ILogExtensions
    {
        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal(this ILog log, Func<object> message)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Fatal(log, message, null);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1>(this ILog log, Func<TIn1, object> message, TIn1 param1)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Fatal(log, message, null, param1);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1, TIn2>(this ILog log, Func<TIn1, TIn2, object> message, TIn1 param1, TIn2 param2)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Fatal(log, message, null, param1, param2);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <typeparam name="TIn3">The <see cref="Type"/> of the third parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="param3">The third parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1, TIn2, TIn3>(this ILog log, Func<TIn1, TIn2, TIn3, object> message, TIn1 param1, TIn2 param2, TIn3 param3)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Fatal(log, message, null, param1, param2, param3);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <typeparam name="TIn3">The <see cref="Type"/> of the third parameter.</typeparam>
        /// <typeparam name="TIn4">The <see cref="Type"/> of the fourth parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="param3">The third parameter to pass to the message generator.</param>
        /// <param name="param4">The fourth parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1, TIn2, TIn3, TIn4>(this ILog log, Func<TIn1, TIn2, TIn3, TIn4, object> message, TIn1 param1, TIn2 param2, TIn3 param3, TIn4 param4)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Fatal(log, message, null, param1, param2, param3, param4);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal(this ILog log, Func<object> message, Exception ex)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Fatal, message, ex);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1>(this ILog log, Func<TIn1, object> message, Exception ex, TIn1 param1)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Fatal, message, ex, param1);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1, TIn2>(this ILog log, Func<TIn1, TIn2, object> message, Exception ex, TIn1 param1, TIn2 param2)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Fatal, message, ex, param1, param2);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <typeparam name="TIn3">The <see cref="Type"/> of the third parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="param3">The third parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1, TIn2, TIn3>(this ILog log, Func<TIn1, TIn2, TIn3, object> message, Exception ex, TIn1 param1, TIn2 param2, TIn3 param3)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Fatal, message, ex, param1, param2, param3);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <typeparam name="TIn3">The <see cref="Type"/> of the third parameter.</typeparam>
        /// <typeparam name="TIn4">The <see cref="Type"/> of the fourth parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="param3">The third parameter to pass to the message generator.</param>
        /// <param name="param4">The fourth parameter to pass to the message generator.</param>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Fatal<TIn1, TIn2, TIn3, TIn4>(this ILog log, Func<TIn1, TIn2, TIn3, TIn4, object> message, Exception ex, TIn1 param1, TIn2 param2, TIn3 param3, TIn4 param4)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Fatal, message, ex, param1, param2, param3, param4);
        }
    }
}
