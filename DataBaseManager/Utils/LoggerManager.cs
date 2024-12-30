using System;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DataBaseManager.Utils {
    public class LoggerManager {
        public ILog Logger { get; set; }

        public LoggerManager(Type type) {
            Logger = LogManager.GetLogger(type);
        }

        public ILog GetLogger(Type type) {
            return LogManager.GetLogger(type);
        }

        public void LogInfo(string message) {
            Logger.Info(message);
        }

        public void LogError(string message, Exception exception) {
            Logger.Error(message, exception);
        }
        public void LogError(string message) {
            Logger.Error(message);
        }

        public void LogFatal(Exception exception) {
            Logger.Fatal(exception);
        }

        public void LogWarn(Exception exception) {
            Logger.Warn(exception);
        }

        public void LogDebug(Exception exception) {
            Logger.Debug(exception);
        }

    }
}