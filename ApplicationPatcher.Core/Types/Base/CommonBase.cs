namespace ApplicationPatcher.Core.Types.Base {
	public class CommonBase<TCommon> where TCommon : CommonBase<TCommon> {
		private bool isLoaded;

		public TCommon Load() {
			var common = (TCommon)this;

			if (isLoaded)
				return common;

			LoadInternal();
			return common;
		}

		protected virtual void LoadInternal() {
			isLoaded = true;
		}
	}
}