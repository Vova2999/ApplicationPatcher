using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.Base {
	// ReSharper disable VirtualMemberNeverOverridden.Global

	public abstract class CommonBase<TCommon, TReflection, TMonoCecil> where TCommon : CommonBase<TCommon, TReflection, TMonoCecil> {
		[UsedImplicitly]
		public abstract string Name { get; }

		[UsedImplicitly]
		public abstract string FullName { get; }

		[UsedImplicitly]
		public virtual TReflection Reflection { get; }

		[UsedImplicitly]
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