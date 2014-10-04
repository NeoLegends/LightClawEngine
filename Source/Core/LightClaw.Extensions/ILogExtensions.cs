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
    /// <summary>
    /// Contains extensions to <see cref="ILog"/>.
    /// </summary>
    public static partial class ILogExtensions
    {
        /// <summary>
        /// Writes the specified message into the <paramref name="logger"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write the <paramref name="message"/> into.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to write into the <paramref name="logger"/>.</param>
        /// <param name="ex">An exception that might have caused the log entry.</param>
        private static void Log(ILogger logger, Level level, Func<object> message, Exception ex)
        {
            Contract.Requires<ArgumentNullException>(level != null);

            if ((logger != null) && logger.IsEnabledFor(level))
            {
                logger.Log(typeof(ILogExtensions), level, message(), ex);
            }
        }

        /// <summary>
        /// Writes the specified message into the <paramref name="logger"/>.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="logger">The <see cref="ILogger"/> to write the <paramref name="message"/> into.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to write into the <paramref name="logger"/>.</param>
        /// <param name="ex">An exception that might have caused the log entry.</param>
        private static void Log<TIn1>(ILogger logger, Level level, Func<TIn1, object> message, Exception ex, TIn1 param1)
        {
            Contract.Requires<ArgumentNullException>(level != null);

            if ((logger != null) && logger.IsEnabledFor(level))
            {
                logger.Log(typeof(ILogExtensions), level, message(param1), ex);
            }
        }

        /// <summary>
        /// Writes the specified message into the <paramref name="logger"/>.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="logger">The <see cref="ILogger"/> to write the <paramref name="message"/> into.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to write into the <paramref name="logger"/>.</param>
        /// <param name="ex">An exception that might have caused the log entry.</param>
        private static void Log<TIn1, TIn2>(ILogger logger, Level level, Func<TIn1, TIn2, object> message, Exception ex, TIn1 param1, TIn2 param2)
        {
            Contract.Requires<ArgumentNullException>(level != null);

            if ((logger != null) && logger.IsEnabledFor(level))
            {
                logger.Log(typeof(ILogExtensions), level, message(param1, param2), ex);
            }
        }

        /// <summary>
        /// Writes the specified message into the <paramref name="logger"/>.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <typeparam name="TIn3">The <see cref="Type"/> of the third parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="param3">The third parameter to pass to the message generator.</param>
        /// <param name="logger">The <see cref="ILogger"/> to write the <paramref name="message"/> into.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to write into the <paramref name="logger"/>.</param>
        /// <param name="ex">An exception that might have caused the log entry.</param>
        private static void Log<TIn1, TIn2, TIn3>(ILogger logger, Level level, Func<TIn1, TIn2, TIn3, object> message, Exception ex, TIn1 param1, TIn2 param2, TIn3 param3)
        {
            Contract.Requires<ArgumentNullException>(level != null);

            if ((logger != null) && logger.IsEnabledFor(level))
            {
                logger.Log(typeof(ILogExtensions), level, message(param1, param2, param3), ex);
            }
        }

        /// <summary>
        /// Writes the specified message into the <paramref name="logger"/>.
        /// </summary>
        /// <typeparam name="TIn1">The <see cref="Type"/> of the first parameter.</typeparam>
        /// <typeparam name="TIn2">The <see cref="Type"/> of the second parameter.</typeparam>
        /// <typeparam name="TIn3">The <see cref="Type"/> of the third parameter.</typeparam>
        /// <typeparam name="TIn4">The <see cref="Type"/> of the fourth parameter.</typeparam>
        /// <param name="param1">The first parameter to pass to the message generator.</param>
        /// <param name="param2">The second parameter to pass to the message generator.</param>
        /// <param name="param3">The third parameter to pass to the message generator.</param>
        /// <param name="param4">The fourth parameter to pass to the message generator.</param>
        /// <param name="logger">The <see cref="ILogger"/> to write the <paramref name="message"/> into.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to write into the <paramref name="logger"/>.</param>
        /// <param name="ex">An exception that might have caused the log entry.</param>
        private static void Log<TIn1, TIn2, TIn3, TIn4>(ILogger logger, Level level, Func<TIn1, TIn2, TIn3, TIn4, object> message, Exception ex, TIn1 param1, TIn2 param2, TIn3 param3, TIn4 param4)
        {
            Contract.Requires<ArgumentNullException>(level != null);

            if ((logger != null) && logger.IsEnabledFor(level))
            {
                logger.Log(typeof(ILogExtensions), level, message(param1, param2, param3, param4), ex);
            }
        }
    }
}
