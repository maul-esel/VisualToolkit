using System;
using System.Windows.Forms;

namespace VisualToolkit
{
	internal class TickOnceTimer : Timer
	{
		private bool ticked = false;

		public bool EnsureOneTick {
			get;
			set;
		}

		public TickOnceTimer(int interval)
			: base()
		{
			EnsureOneTick = true;
			Interval = interval;
		}

		protected override void OnTick(EventArgs e)
		{
			ticked = true;
			base.OnTick(e);
		}

		public new void Stop()
		{
			if (Enabled) {
				base.Stop();
				if (!ticked && EnsureOneTick)
					OnTick(EventArgs.Empty);
				ticked = false;
			}
		}
	}
}