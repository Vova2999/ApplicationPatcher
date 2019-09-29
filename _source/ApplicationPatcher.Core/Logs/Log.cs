using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using log4net;
using log4net.Config;
using log4net.Core;

namespace ApplicationPatcher.Core.Logs {
	public class Log : ILog {
		static Log() {
			XmlConfigurator.Configure();
		}

		public static ILog For<TObject>(TObject _) {
			return For(typeof(TObject));
		}
		public static ILog For(Type type) {
			return new Log(LoggerManager.GetLogger(Assembly.GetExecutingAssembly(), type));
		}

		public static int DefaultOffset { get; set; }

		public bool IsDebugEnabled => Logger.IsEnabledFor(Logger.Repository.LevelMap.LookupWithDefault(Level.Debug));
		public bool IsInfoEnabled => Logger.IsEnabledFor(Logger.Repository.LevelMap.LookupWithDefault(Level.Info));
		public bool IsWarnEnabled => Logger.IsEnabledFor(Logger.Repository.LevelMap.LookupWithDefault(Level.Warn));
		public bool IsErrorEnabled => Logger.IsEnabledFor(Logger.Repository.LevelMap.LookupWithDefault(Level.Error));
		public bool IsFatalEnabled => Logger.IsEnabledFor(Logger.Repository.LevelMap.LookupWithDefault(Level.Fatal));

		public ILogger Logger { get; }

		private static string OffsetString {
			get => (string)GlobalContext.Properties["Offset"];
			set => GlobalContext.Properties["Offset"] = value;
		}

		private Log(ILogger logger) {
			Logger = logger;
		}

		public void Debug(object message) {
			SetOffsetAndExecuteLog(Level.Debug, () => FixMultiline(message));
		}
		public void Debug(Exception exception) {
			SetOffsetAndExecuteLog(Level.Debug, () => null, exception);
		}
		public void Debug(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Debug, () => FixMultiline(message), exception);
		}
		public void Debug(object message, IEnumerable<object> messages) {
			SetOffsetAndExecuteLog(Level.Debug, () => JoinMultiline(message, messages));
		}
		public void DebugFormat(string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Debug, () => FormatMultiline(format, args));
		}
		public void DebugFormat(string format, object arg0) {
			SetOffsetAndExecuteLog(Level.Debug, () => FormatMultiline(format, arg0));
		}
		public void DebugFormat(string format, object arg0, object arg1) {
			SetOffsetAndExecuteLog(Level.Debug, () => FormatMultiline(format, arg0, arg1));
		}
		public void DebugFormat(string format, object arg0, object arg1, object arg2) {
			SetOffsetAndExecuteLog(Level.Debug, () => FormatMultiline(format, arg0, arg1, arg2));
		}
		public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Debug, () => FormatMultiline(provider, format, args));
		}

		public void Info(object message) {
			SetOffsetAndExecuteLog(Level.Info, () => FixMultiline(message));
		}
		public void Info(Exception exception) {
			SetOffsetAndExecuteLog(Level.Info, () => null, exception);
		}
		public void Info(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Info, () => FixMultiline(message), exception);
		}
		public void Info(object message, IEnumerable<object> messages) {
			SetOffsetAndExecuteLog(Level.Info, () => JoinMultiline(message, messages));
		}
		public void InfoFormat(string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Info, () => FormatMultiline(format, args));
		}
		public void InfoFormat(string format, object arg0) {
			SetOffsetAndExecuteLog(Level.Info, () => FormatMultiline(format, arg0));
		}
		public void InfoFormat(string format, object arg0, object arg1) {
			SetOffsetAndExecuteLog(Level.Info, () => FormatMultiline(format, arg0, arg1));
		}
		public void InfoFormat(string format, object arg0, object arg1, object arg2) {
			SetOffsetAndExecuteLog(Level.Info, () => FormatMultiline(format, arg0, arg1, arg2));
		}
		public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Info, () => FormatMultiline(provider, format, args));
		}

		public void Warn(object message) {
			SetOffsetAndExecuteLog(Level.Warn, () => FixMultiline(message));
		}
		public void Warn(Exception exception) {
			SetOffsetAndExecuteLog(Level.Warn, () => null, exception);
		}
		public void Warn(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Warn, () => FixMultiline(message), exception);
		}
		public void Warn(object message, IEnumerable<object> messages) {
			SetOffsetAndExecuteLog(Level.Warn, () => JoinMultiline(message, messages));
		}
		public void WarnFormat(string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Warn, () => FormatMultiline(format, args));
		}
		public void WarnFormat(string format, object arg0) {
			SetOffsetAndExecuteLog(Level.Warn, () => FormatMultiline(format, arg0));
		}
		public void WarnFormat(string format, object arg0, object arg1) {
			SetOffsetAndExecuteLog(Level.Warn, () => FormatMultiline(format, arg0, arg1));
		}
		public void WarnFormat(string format, object arg0, object arg1, object arg2) {
			SetOffsetAndExecuteLog(Level.Warn, () => FormatMultiline(format, arg0, arg1, arg2));
		}
		public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Warn, () => FormatMultiline(provider, format, args));
		}

		public void Error(object message) {
			SetOffsetAndExecuteLog(Level.Error, () => FixMultiline(message));
		}
		public void Error(Exception exception) {
			SetOffsetAndExecuteLog(Level.Error, () => null, exception);
		}
		public void Error(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Error, () => FixMultiline(message), exception);
		}
		public void Error(object message, IEnumerable<object> messages) {
			SetOffsetAndExecuteLog(Level.Error, () => JoinMultiline(message, messages));
		}
		public void ErrorFormat(string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Error, () => FormatMultiline(format, args));
		}
		public void ErrorFormat(string format, object arg0) {
			SetOffsetAndExecuteLog(Level.Error, () => FormatMultiline(format, arg0));
		}
		public void ErrorFormat(string format, object arg0, object arg1) {
			SetOffsetAndExecuteLog(Level.Error, () => FormatMultiline(format, arg0, arg1));
		}
		public void ErrorFormat(string format, object arg0, object arg1, object arg2) {
			SetOffsetAndExecuteLog(Level.Error, () => FormatMultiline(format, arg0, arg1, arg2));
		}
		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Error, () => FormatMultiline(provider, format, args));
		}

		public void Fatal(object message) {
			SetOffsetAndExecuteLog(Level.Fatal, () => FixMultiline(message));
		}
		public void Fatal(Exception exception) {
			SetOffsetAndExecuteLog(Level.Fatal, () => null, exception);
		}
		public void Fatal(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Fatal, () => FixMultiline(message), exception);
		}
		public void Fatal(object message, IEnumerable<object> messages) {
			SetOffsetAndExecuteLog(Level.Fatal, () => JoinMultiline(message, messages));
		}
		public void FatalFormat(string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Fatal, () => FormatMultiline(format, args));
		}
		public void FatalFormat(string format, object arg0) {
			SetOffsetAndExecuteLog(Level.Fatal, () => FormatMultiline(format, arg0));
		}
		public void FatalFormat(string format, object arg0, object arg1) {
			SetOffsetAndExecuteLog(Level.Fatal, () => FormatMultiline(format, arg0, arg1));
		}
		public void FatalFormat(string format, object arg0, object arg1, object arg2) {
			SetOffsetAndExecuteLog(Level.Fatal, () => FormatMultiline(format, arg0, arg1, arg2));
		}
		public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
			SetOffsetAndExecuteLog(Level.Fatal, () => FormatMultiline(provider, format, args));
		}

		private void SetOffsetAndExecuteLog(Level level, Func<string> message, Exception exception = null) {
			var stackMethods = (new StackTrace().GetFrames() ?? throw new Exception()).Select(x => x.GetMethod()).ToArray();
			var offset = stackMethods.Count(method => method.GetCustomAttribute<AddLogOffsetAttribute>() != null);

			OffsetString = new string('\t', Math.Max(offset + DefaultOffset, 0));
			Logger.Log(stackMethods.First().DeclaringType, level, message?.Invoke(), exception);
		}
		private static string FixMultiline(object message) {
			return message.ToString().Replace("\n", $"\r\n{OffsetString}");
		}
		private static string JoinMultiline(object message, IEnumerable<object> messages) {
			return FixMultiline(new[] { message }.Concat(messages?.Select((m, i) => $"  {i + 1}) {m}") ?? Enumerable.Empty<string>()).JoinToString("\n"));
		}
		private static string FormatMultiline(string format, params object[] args) {
			return string.Format(FixMultiline(format), args);
		}
		private static string FormatMultiline(string format, object arg0) {
			return string.Format(FixMultiline(format), arg0);
		}
		private static string FormatMultiline(string format, object arg0, object arg1) {
			return string.Format(FixMultiline(format), arg0, arg1);
		}
		private static string FormatMultiline(string format, object arg0, object arg1, object arg2) {
			return string.Format(FixMultiline(format), arg0, arg1, arg2);
		}
		private static string FormatMultiline(IFormatProvider provider, string format, params object[] args) {
			return string.Format(provider, FixMultiline(format), args);
		}
	}
}