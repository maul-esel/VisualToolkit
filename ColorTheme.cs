using System.Drawing;
using System.Windows.Forms;

namespace VisualToolkit
{
	public class ColorTheme
	{
		private static ColorTheme defaultTheme = new ColorTheme() {
			TextColor = Color.Black,
			BackColor = Color.White,
			SymbolColor = Color.Gray,
			BorderColor = Color.Gray
		};

		public static ColorTheme DefaultTheme {
			get { return defaultTheme; }
			set { defaultTheme = value; }
		}

		private static ColorTheme highlightTheme = new ColorTheme() {
			TextColor = Color.Black,
			BackColor = Color.White,
			SymbolColor = ControlPaint.Dark(Color.Gray, 0f),
			BorderColor = ControlPaint.Dark(Color.Gray, 0f)
		};

		public static ColorTheme HighlightTheme {
			get { return highlightTheme; }
			set { highlightTheme = value; }
		}

		public Color TextColor { get; set; }
		public Color BackColor { get; set; }
		public Color SymbolColor { get; set; }
		public Color BorderColor { get; set; }
	}
}