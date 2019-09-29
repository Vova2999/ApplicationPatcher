using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Logs {
	[PublicAPI]
	public interface ILog : log4net.ILog {
		void Debug(Exception exception);
		void Debug(object message, IEnumerable<object> messages);
		void Info(Exception exception);
		void Info(object message, IEnumerable<object> messages);
		void Warn(Exception exception);
		void Warn(object message, IEnumerable<object> messages);
		void Error(Exception exception);
		void Error(object message, IEnumerable<object> messages);
		void Fatal(Exception exception);
		void Fatal(object message, IEnumerable<object> messages);
	}
}