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
    public static class ILogExtensions
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Debug(this ILog log, Func<object> message)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Debug(log, message, null);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Debug(this ILog log, Func<object> message, Exception ex)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Debug, message, ex);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Info(this ILog log, Func<object> message)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Info(log, message, null);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the message into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Info(this ILog log, Func<object> message, Exception ex)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Info, message, ex);
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the warning into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Warn(this ILog log, Func<object> message)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Warn(log, message, null);
        }

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the warning into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Warn(this ILog log, Func<object> message, Exception ex)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Warn, message, ex);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the error into.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Error(this ILog log, Func<object> message)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Error(log, message, null);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the error into.</param>
        /// <param name="ex">An <see cref="Exception"/> that will be logged with the entry.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// The extension method accepts a <see cref="Func{T}"/> instead of the message directly to avoid (a possibly expensive) 
        /// creation of the log message if the targeted log level isn't even enabled.
        /// </remarks>
        public static void Error(this ILog log, Func<object> message, Exception ex)
        {
            Contract.Requires<ArgumentNullException>(log != null);

            Log(log.Logger, Level.Error, message, ex);
        }

        /// <summary>
        /// Logs a fatal error.
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> to log the error into.</param>
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
        /// <param name="log">The <see cref="ILog"/> to log the error into.</param>
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
    }
}
