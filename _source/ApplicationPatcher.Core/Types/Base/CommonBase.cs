using System;
using System.Collections.Concurrent;
using System.Diagnostics;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace ApplicationPatcher.Core.Types.Base {
	public abstract class CommonBase<TCommon, TReflection, TMonoCecil> where TCommon : CommonBase<TCommon, TReflection, TMonoCecil> {
		public abstract string Name { get; }
		public abstract string FullName { get; }

		public virtual TReflection Reflection { get; }
		public virtual TMonoCecil MonoCecil { get; }

		private bool isLoaded;
		private readonly ConcurrentDictionary<string, object> values = new ConcurrentDictionary<string, object>();

		protected CommonBase(TReflection reflection, TMonoCecil monoCecil) {
			Reflection = reflection;
			MonoCecil = monoCecil;
		}

		public virtual TCommon Load() {
			var common = (TCommon)this;

			if (isLoaded)
				return common;

			LoadInternal();
			return common;
		}

		protected virtual void LoadInternal() {
			isLoaded = true;
		}

		protected TValue GetOrCreate<TValue>(Func<TValue> value) {
			return (TValue)values.GetOrAdd(new StackTrace().GetFrame(1).GetMethod().Name, _ => value());
		}
	}
}