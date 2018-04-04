using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using log4net.Core;

namespace ApplicationPatcher.Core.Helpers {
	public class Log {
		private static readonly List<Module> modules = new List<Module>();

		private static string OffsetString {
			get => (string)GlobalContext.Properties["Offset"];
			set => GlobalContext.Properties["Offset"] = value;
		}

		private readonly ILogger logger;

		private Log(ILogger logger) {
			this.logger = logger;
		}

		static Log() {
			XmlConfigurator.Configure();
		}

		[UsedImplicitly]
		public static Log For<TObject>(TObject obj) {
			modules.AddRange(Assembly.GetAssembly(typeof(TObject)).Modules);
			return new Log(LoggerManager.GetLogger(Assembly.GetExecutingAssembly(), typeof(TObject)));
		}

		[UsedImplicitly]
		public void Debug(string message) {
			SetOffsetAndExecuteLog(Level.Debug, () => ConvertMultiline(message));
		}
		[UsedImplicitly]
		public void Debug(Exception exception) {
			SetOffsetAndExecuteLog(Level.Debug, () => null, exception);
		}
		[UsedImplicitly]
		public void Debug(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Debug, () => ConvertMultiline(message), exception);
		}
		[UsedImplicitly]
		public void Debug(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Debug, () => JoinMultiline(message, messages));
		}

		[UsedImplicitly]
		public void Info(string message) {
			SetOffsetAndExecuteLog(Level.Info, () => ConvertMultiline(message));
		}
		[UsedImplicitly]
		public void Info(Exception exception) {
			SetOffsetAndExecuteLog(Level.Info, () => null, exception);
		}
		[UsedImplicitly]
		public void Info(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Info, () => ConvertMultiline(message), exception);
		}
		[UsedImplicitly]
		public void Info(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Info, () => JoinMultiline(message, messages));
		}

		[UsedImplicitly]
		public void Warn(string message) {
			SetOffsetAndExecuteLog(Level.Warn, () => ConvertMultiline(message));
		}
		[UsedImplicitly]
		public void Warn(Exception exception) {
			SetOffsetAndExecuteLog(Level.Warn, () => null, exception);
		}
		[UsedImplicitly]
		public void Warn(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Warn, () => ConvertMultiline(message), exception);
		}
		[UsedImplicitly]
		public void Warn(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Warn, () => JoinMultiline(message, messages));
		}

		[UsedImplicitly]
		public void Error(string message) {
			SetOffsetAndExecuteLog(Level.Error, () => ConvertMultiline(message));
		}
		[UsedImplicitly]
		public void Error(Exception exception) {
			SetOffsetAndExecuteLog(Level.Error, () => null, exception);
		}
		[UsedImplicitly]
		public void Error(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Error, () => ConvertMultiline(message), exception);
		}
		[UsedImplicitly]
		public void Error(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Error, () => JoinMultiline(message, messages));
		}

		[UsedImplicitly]
		public void Fatal(string message) {
			SetOffsetAndExecuteLog(Level.Fatal, () => ConvertMultiline(message));
		}
		[UsedImplicitly]
		public void Fatal(Exception exception) {
			SetOffsetAndExecuteLog(Level.Fatal, () => null, exception);
		}
		[UsedImplicitly]
		public void Fatal(object message, Exception exception) {
			SetOffsetAndExecuteLog(Level.Fatal, () => ConvertMultiline(message), exception);
		}
		[UsedImplicitly]
		public void Fatal(string message, IEnumerable<string> messages) {
			SetOffsetAndExecuteLog(Level.Fatal, () => JoinMultiline(message, messages));
		}

		private void SetOffsetAndExecuteLog(Level level, Func<string> message, Exception exception = null) {
			var stackMethods = (new StackTrace().GetFrames() ?? throw new Exception()).Select(x => x.GetMethod()).ToArray();
			var offset = stackMethods.Count(method => method.DeclaringType != typeof(Log) && modules.Contains(method.Module) && method.GetCustomAttribute<DoNotAddLogOffsetAttribute>() == null);

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
	public class DoNotAddLogOffsetAttribute : Attribute {
	}
}