using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

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

            if (log.IsInfoEnabled)
            {
                log.Debug(message());
            }
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

            if (log.IsInfoEnabled)
            {
                log.Debug(message(), ex);
            }
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

            if (log.IsInfoEnabled)
            {
                log.Info(message());
            }
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

            if (log.IsInfoEnabled)
            {
                log.Info(message(), ex);
            }
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

            if (log.IsInfoEnabled)
            {
                log.Warn(message());
            }
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

            if (log.IsInfoEnabled)
            {
                log.Warn(message(), ex);
            }
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

            if (log.IsInfoEnabled)
            {
                log.Error(message());
            }
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

            if (log.IsInfoEnabled)
            {
                log.Error(message(), ex);
            }
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

            if (log.IsInfoEnabled)
            {
                log.Fatal(message());
            }
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

            if (log.IsInfoEnabled)
            {
                log.Fatal(message(), ex);
            }
        }
    }
}
