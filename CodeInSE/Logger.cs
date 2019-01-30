using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IngameScript
{
    partial class Program
    {
        public class Logger
        {
            public enum LogLevel
            {
                // System level logs.
                SYSTEM = 0,
                // Debugging logs.
                DEBUG = 1,
                // General output should be sent through info.
                INFO = 3,
                // Notification logging.
                WARNING = 7,
                // Error logging.
                ERROR = 10
            }

            private Program _program;
            private string _globalIdentifier;
            private LogLevel _logLevel;
            private bool _appendLogs;
            private bool _clearLogPerCommand;
            private IDictionary<string, string> _logMessageHeaders;
            private IDictionary<string, IDictionary<int, string>> _logMessages;
            private IDictionary<string, IDictionary<int, LogLevel>> _logLevels;

            /// <summary>
            /// Creates a logger for your program.
            /// </summary>
            /// <param name="logLevel">The level of logging you want output. The lower the level the more logs that will be shown.</param>
            /// <param name="globalIdentifier">Used to identify a destination for all logs. Logger will output to the PB, but will also attempt to find a lcd panel by this name or group name.</param>
            /// <param name="appendLogs">If set to true, will append logs rather than overwrite them (per script execution).</param>
            /// <param name="clearLogPerCommand">If set to true, will clear logs each time a command is run giving a new output per script execution.</param>
            public Logger(
                LogLevel logLevel = LogLevel.DEBUG,
                string globalIdentifier = "global",
                bool appendLogs = true,
                bool clearLogPerCommand = true
            )
            {
                _program = Program.program;
                _globalIdentifier = globalIdentifier;
                _logLevel = logLevel;
                _appendLogs = appendLogs;
                _clearLogPerCommand = clearLogPerCommand;
                RenewLog();
                OutputLogMessage(new KeyValuePair<string, string>(_globalIdentifier, ""));
                SysLog<Logger>("initialized.");
            }

            /// <summary>
            /// Append a message to the global log body and optionally to the log body of the given logIdentifier.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="logIdentifier"></param>
            /// <param name="logLevel"></param>
            public void AppendToLogBody(string message, string logIdentifier = "", LogLevel logLevel = LogLevel.INFO)
            {
                string lineHeader = GetLogLineHeader(logLevel);
                // If a logIdentifier was provided, log the message under the logIdentifier separately.
                if (!String.IsNullOrEmpty(logIdentifier))
                {
                    if (!_logMessages.ContainsKey(logIdentifier))
                    {
                        _logMessages[logIdentifier] = new Dictionary<int, string>();
                        _logLevels[logIdentifier] = new Dictionary<int, LogLevel>();
                    }
                    _logMessages[logIdentifier].Add(_logMessages[logIdentifier].Count, $"\n{lineHeader}{message}");
                    _logLevels[logIdentifier].Add(_logLevels[logIdentifier].Count, logLevel);
                    logIdentifier = logIdentifier + ": ";
                }

                if (!_logMessages.ContainsKey(_globalIdentifier))
                {
                    _logMessages[_globalIdentifier] = new Dictionary<int, string>();
                    _logLevels[_globalIdentifier] = new Dictionary<int, LogLevel>();
                }
                _logMessages[_globalIdentifier].Add(_logMessages[_globalIdentifier].Count, $"\n{lineHeader}{logIdentifier}{message}");
                _logLevels[_globalIdentifier].Add(_logLevels[_globalIdentifier].Count, logLevel);
            }

            /// <summary>
            /// Shorthand function for logging system messages.
            /// </summary>
            /// <param name="message"></param>
            public void SysLog<T>(string message)
            {
                this.AppendToLogBody($"{typeof(T).Name}: {message}", "", LogLevel.SYSTEM);
            }

            /// <summary>
            /// Sets a header string to the given logIdentifier. Defaults to the global identifier.
            /// </summary>
            /// <param name="header"></param>
            /// <param name="logIdentifier"></param>
            public void SetLogHeader(string header, string logIdentifier = "")
            {
                if (String.IsNullOrEmpty(logIdentifier))
                {
                    logIdentifier = _globalIdentifier;
                }

                if (_logMessageHeaders.ContainsKey(logIdentifier))
                {
                    _logMessageHeaders[logIdentifier] = header;
                }
                else
                {
                    _logMessageHeaders.Add(logIdentifier, header);
                }
            }

            /// <summary>
            /// Removes any entries within the log messages or log headers.
            /// </summary>
            public void RenewLog()
            {
                _logMessageHeaders = new Dictionary<string, string>();
                SetLogHeader($"== {_globalIdentifier} ==\n______________________\n");
                _logMessages = new Dictionary<string, IDictionary<int, string>>();
                _logMessages[_globalIdentifier] = new Dictionary<int, string>();
                _logLevels = new Dictionary<string, IDictionary<int, LogLevel>>();
                _logLevels[_globalIdentifier] = new Dictionary<int, LogLevel>();
            }

            /// <summary>
            /// Output Logger's current state to PB and applicable lcd panels.
            /// </summary>
            public void Log()
            {
                foreach (KeyValuePair<string, string> logMessageHeader in _logMessageHeaders)
                {
                    string output = BuildLogMessage(logMessageHeader);
                    OutputLogMessage(logMessageHeader, output);
                }

                if (!_appendLogs || (_clearLogPerCommand && !_program.isSelfUpdate()))
                {
                    RenewLog();
                }
            }

            private string BuildLogMessage(KeyValuePair<string, string> logMessageHeader)
            {
                string header = logMessageHeader.Value;
                string logMessageStr = "";
                if (_logMessages.ContainsKey(logMessageHeader.Key))
                {
                    Dictionary<int, string> logMessages = (Dictionary<int, string>)_logMessages[logMessageHeader.Key];
                    foreach (KeyValuePair<int, string> logMessage in logMessages)
                    {
                        if (
                            _logLevels.ContainsKey(logMessageHeader.Key) &&
                            _logLevels[logMessageHeader.Key].ContainsKey(logMessage.Key) &&
                            _logLevels[logMessageHeader.Key][logMessage.Key] >= _logLevel)
                        {
                            logMessageStr += logMessage.Value;
                        }
                    }
                }

                return $"{header}{logMessageStr}";
            }

            private void OutputLogMessage(KeyValuePair<string, string> logMessageHeader, string output = "")
            {
                if (logMessageHeader.Key == _globalIdentifier)
                    _program.Echo(output);

                foreach (IMyTextPanel panel in _program._grid.GetBlocksOfTypeFromNameOrGroup<IMyTextPanel>(logMessageHeader.Key))
                {
                    panel.ShowPublicTextOnScreen();
                    panel.WritePublicText(output, false);
                }
            }

            /// <summary>
            /// Returns a formatted time string for log events.
            /// </summary>
            /// <returns>string</returns>
            private string GetLogTime()
            {
                return "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "]";
            }

            private string GetLogLineHeader(LogLevel logLevel)
            {
                return GetLogTime() + $" [{logLevel.ToString()}]:\n";
            }
        }
    }
}
