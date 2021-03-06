﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types {
	[PublicAPI]
	public abstract class CommonMember<TCommonMember, TMonoCecil, TReflection> : ICommonMember<TCommonMember, TMonoCecil, TReflection>
		where TCommonMember : ICommonMember<TCommonMember, TMonoCecil, TReflection> {
		public abstract string Name { get; }
		public abstract string FullName { get; }

		public TMonoCecil MonoCecil { get; }
		public TReflection Reflection { get; }

		private bool isLoaded;
		private readonly ConcurrentDictionary<string, object> values;

		protected CommonMember(TMonoCecil monoCecil, TReflection reflection) {
			MonoCecil = monoCecil;
			Reflection = reflection;
			values = new ConcurrentDictionary<string, object>();
		}

		public TCommonMember Load(int depth = 0) {
			if (!isLoaded) {
				LoadInternal();
				isLoaded = true;
			}

			if (depth != 0)
				LoadInDepth(depth - 1);

			return (TCommonMember)(ICommonMember<TCommonMember, TMonoCecil, TReflection>)this;
		}

		protected abstract void LoadInternal();
		protected abstract void LoadInDepth(int depth);

		protected TValue GetOrCreate<TValue>(Func<TValue> value) {
			return (TValue)values.GetOrAdd(new StackTrace().GetFrame(1).GetMethod().Name, _ => value());
		}
	}
}