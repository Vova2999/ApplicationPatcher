using System;
using System.Collections.Concurrent;
using System.Diagnostics;

// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types {
	public abstract class CommonMemberBase<TCommon, TReflection, TMonoCecil> where TCommon : CommonMemberBase<TCommon, TReflection, TMonoCecil> {
		public abstract string Name { get; }
		public abstract string FullName { get; }

		public TReflection Reflection { get; }
		public TMonoCecil MonoCecil { get; }

		private bool isLoaded;
		private readonly ConcurrentDictionary<string, object> values = new ConcurrentDictionary<string, object>();

		protected CommonMemberBase(TReflection reflection, TMonoCecil monoCecil) {
			Reflection = reflection;
			MonoCecil = monoCecil;
		}

		public TCommon Load() {
			var common = (TCommon)this;

			if (isLoaded)
				return common;

			LoadInternal();
			return common;
		}

		internal virtual void LoadInternal() {
			isLoaded = true;
		}

		protected TValue GetOrCreate<TValue>(Func<TValue> value) {
			return (TValue)values.GetOrAdd(new StackTrace().GetFrame(1).GetMethod().Name, _ => value());
		}
	}
}