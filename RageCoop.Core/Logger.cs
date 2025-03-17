using GTA.NaturalMotion;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace RageCoop.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Logger : IDisposable
    {
        /// <summary>
        /// Log levels
        /// </summary>
        internal enum LogLevels
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error    
        }

        /// <summary>
        /// 0:Trace, 1:Debug, 2:Info, 3:Warning, 4:Error
        /// </summary>
        internal LogLevels LogLevel = 0;
        /// <summary>
        /// Name of this logger
        /// </summary>
        internal string Name { get; set; }
        /// <summary>
        /// Path to log file.
        /// </summary>
        internal string LogPath;
        /// <summary>
        /// Whether to flush messages to console instead of log file
        /// </summary>
        internal bool UseConsole = false;
        private StreamWriter logWriter;

        private string Buffer = "";
        private readonly Thread LoggerThread;
        private bool Stopping = false;
        private readonly bool FlushImmediately;

        internal Logger(bool flushImmediately = false, bool overwrite = true)
        {
            FlushImmediately = flushImmediately;
            if (File.Exists(LogPath) && overwrite) { try { File.Delete(LogPath); } catch { } }
            Name = Process.GetCurrentProcess().Id.ToString();
            if (!flushImmediately)
            {
                LoggerThread = new Thread(() =>
                  {
                      if (!UseConsole)
                      {
                          while (LogPath == default)
                          {
                              Thread.Sleep(100);
                          }
                          if (File.Exists(LogPath) && overwrite) { try { File.Delete(LogPath); } catch { } }
                      }
                      while (!Stopping)
                      {
                          Flush();
                          Thread.Sleep(1000);
                      }
                      Flush();
                  });
                LoggerThread.Start();
            }
        }

        private void WriteLine(string message, string logLevel)
        {
            string logLine = $"[{DateTime.Now.ToString("yyMMdd HH:mm:ss.fff")}][{Name}] - [{logLevel}] - {message}\n";
            lock (Buffer)
            {
                Buffer += logLine;
            }
            if (FlushImmediately)
            {
                Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            if (LogLevel <= LogLevels.Info) 
            {
                WriteLine(message, "I");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warning(string message)
        {
            if (LogLevel <= LogLevels.Warning)
            {
                WriteLine(message, "W");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            if (LogLevel <= LogLevels.Error)
            {
                WriteLine(message, "E");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public void Error(string message, Exception error)
        {
            if (LogLevel <= LogLevels.Error)
            {
                message = $"{message}:{error.Message}";
                WriteLine(message, "E");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public void Error(Exception ex)
        {
            if (LogLevel <= LogLevels.Error)
            {
                WriteLine(ex.Message, "E");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            if (LogLevel <= LogLevels.Debug)
            {
                WriteLine(message, "D");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message)
        {
            if (LogLevel <= LogLevels.Trace)
            {
                WriteLine(message, "T");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
            lock (Buffer)
            {
                if (Buffer != "")
                {
                    if (UseConsole)
                    {
                        Console.Write(Buffer);
                        Buffer = "";
                    }
                    else
                    {
                        try
                        {
                            logWriter = new StreamWriter(LogPath, true, Encoding.UTF8);
                            logWriter.Write(Buffer);
                            logWriter.Close();
                            Buffer = "";
                        }
                        catch { }
                    }
                }

            }
        }
        /// <summary>
        /// Stop backdround thread and flush all pending messages.
        /// </summary>
        public void Dispose()
        {
            Stopping = true;
            LoggerThread?.Join();
        }
    }
}
