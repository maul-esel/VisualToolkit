using System.Windows.Forms;

namespace VisualToolkit
{
	public abstract class Separator : Label
	{
		public override bool AutoSize {
			get { return false; }
		}

		public override BorderStyle BorderStyle {
			get { return BorderStyle.Fixed3D; }
		}
	}
}