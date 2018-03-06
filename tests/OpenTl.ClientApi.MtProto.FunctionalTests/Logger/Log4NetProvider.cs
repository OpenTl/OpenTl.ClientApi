// namespace OpenTl.ClientApi.MtProto.FunctionalTests.Logger
// {
//     using System;
//     using System.Collections.Concurrent;
//     using System.IO;
//     using System.Xml;
//
//     using log4net;
//
//     using Microsoft.Extensions.Logging;
//
//     public class Log4NetLogger : ILogger
//     {
//         private readonly ILog _log;
//
//         public Log4NetLogger(Type type)
//         {
//             _log = LogManager.GetLogger(type);
//         }
//
//         public IDisposable BeginScope<TState>(TState state)
//         {
//             return null;
//         }
//
//         public bool IsEnabled(LogLevel logLevel)
//         {
//             switch (logLevel)
//             {
//                 case LogLevel.Critical:
//                     return _log.IsFatalEnabled;
//                 case LogLevel.Debug:
//                 case LogLevel.Trace:
//                     return _log.IsDebugEnabled;
//                 case LogLevel.Error:
//                     return _log.IsErrorEnabled;
//                 case LogLevel.Information:
//                     return _log.IsInfoEnabled;
//                 case LogLevel.Warning:
//                     return _log.IsWarnEnabled;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(logLevel));
//             }
//         }
//
//         public void Log<TState>(LogLevel logLevel,
//                                 EventId eventId,
//                                 TState state,
//                                 Exception exception,
//                                 Func<TState, Exception, string> formatter)
//         {
//             if (!IsEnabled(logLevel))
//             {
//                 return;
//             }
//
//             if (formatter == null)
//             {
//                 throw new ArgumentNullException(nameof(formatter));
//             }
//
//             var message = formatter(state, exception);
//             if (!string.IsNullOrEmpty(message) || exception != null)
//             {
//                 switch (logLevel)
//                 {
//                     case LogLevel.Critical:
//                         _log.Fatal(message);
//                         break;
//                     case LogLevel.Debug:
//                     case LogLevel.Trace:
//                         _log.Debug(message);
//                         break;
//                     case LogLevel.Error:
//                         _log.Error(message);
//                         break;
//                     case LogLevel.Information:
//                         _log.Info(message);
//                         break;
//                     case LogLevel.Warning:
//                         _log.Warn(message);
//                         break;
//                     default:
//                         _log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
//                         _log.Info(message, exception);
//                         break;
//                 }
//             }
//         }
//     }
//
//     public class Log4NetProvider : ILoggerProvider
//     {
//         private readonly string _log4NetConfigFile;
//
//         private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers =
//             new ConcurrentDictionary<string, Log4NetLogger>();
//
//         public Log4NetProvider(string log4NetConfigFile)
//         {
//             _log4NetConfigFile = log4NetConfigFile;
//         }
//
//         public ILogger CreateLogger(string categoryName)
//         {
//             return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
//         }
//
//         public void Dispose()
//         {
//             _loggers.Clear();
//         }
//
//         private Log4NetLogger CreateLoggerImplementation(Type name)
//         {
//             return new Log4NetLogger(name);
//         }
//
//         private static XmlElement Parselog4NetConfigFile(string filename)
//         {
//             var log4NetConfig = new XmlDocument();
//             log4NetConfig.Load(File.OpenRead(filename));
//             return log4NetConfig["log4net"];
//         }
//     }
// }