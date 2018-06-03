using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using log4net.Core;

// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Helpers {
	public class Log {
		private readonly ILogger logger;

		private static string OffsetString {
			get => (string)GlobalContext.Properties["Offset"];
			set => GlobalContext.Properties["Offset"] = value;
		}

		private Log(ILogger logger) {
			this.logger = logger;
		}

		static Log() {
			XmlConfigurator.Configure();
		}

		[UsedImplicitly]
		public static Log For<TObject>(TObject obj) {
			return new Log(LoggerManager.GetLogger(Assembly.GetExecutingAssembly(), typeof(TObject)));
		}

		public void Debug(string message) {
			SetOffsetAndExecuteLog(Level.Debug, () => ConvertMultiline(message));
		}
		public void Debug(Exception exception) {
			SetOffsetAndExecuteLog(Level.Debug, () => null, exception);
		}
		public void Debug(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Debug, () => ConvertMultiline(message), exception);
		}
		public void Debug(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Debug, () => JoinMultiline(message, messages));
		}

		public void Info(string message) {
			SetOffsetAndExecuteLog(Level.Info, () => ConvertMultiline(message));
		}
		public void Info(Exception exception) {
			SetOffsetAndExecuteLog(Level.Info, () => null, exception);
		}
		public void Info(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Info, () => ConvertMultiline(message), exception);
		}
		public void Info(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Info, () => JoinMultiline(message, messages));
		}

		public void Warn(string message) {
			SetOffsetAndExecuteLog(Level.Warn, () => ConvertMultiline(message));
		}
		public void Warn(Exception exception) {
			SetOffsetAndExecuteLog(Level.Warn, () => null, exception);
		}
		public void Warn(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Warn, () => ConvertMultiline(message), exception);
		}
		public void Warn(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Warn, () => JoinMultiline(message, messages));
		}

		public void Error(string message) {
			SetOffsetAndExecuteLog(Level.Error, () => ConvertMultiline(message));
		}
		public void Error(Exception exception) {
			SetOffsetAndExecuteLog(Level.Error, () => null, exception);
		}
		public void Error(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Error, () => ConvertMultiline(message), exception);
		}
		public void Error(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Error, () => JoinMultiline(message, messages));
		}

		public void Fatal(string message) {
			SetOffsetAndExecuteLog(Level.Fatal, () => ConvertMultiline(message));
		}
		public void Fatal(Exception exception) {
			SetOffsetAndExecuteLog(Level.Fatal, () => null, exception);
		}
		public void Fatal(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Fatal, () => ConvertMultiline(message), exception);
		}
		public void Fatal(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Fatal, () => JoinMultiline(message, messages));
		}

		private void SetOffsetAndExecuteLog(Level level, Func<string> message, Exception exception = null) {
			var stackMethods = (new StackTrace().GetFrames() ?? throw new Exception()).Select(x => x.GetMethod()).ToArray();
			var offset = stackMethods.Count(method => method.GetCustomAttribute<AddLogOffsetAttribute>() != null);

			OffsetString = new string('\t', offset);
			logger.Log(stackMethods.First().DeclaringType, level, message?.Invoke(), exception);
		}
		private static string ConvertMultiline(object message) {
			return message.ToString().Replace("\n", $"\r\n{OffsetString}");
		}
		private static string JoinMultiline(string message, IEnumerable<string> messages) {
			return ConvertMultiline(string.Join("\n", new[] { message }.Concat(messages?.Select((m, i) => $"  {i + 1}) {m}") ?? Enumerable.Empty<string>())));
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class AddLogOffsetAttribute : Attribute {
	}
}