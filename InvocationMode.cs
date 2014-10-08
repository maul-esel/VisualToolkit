using System;

namespace VisualToolkit
{
	[Flags]
	public enum InvocationMode
	{
		None = 0,
		Select = 1,
		Click = 2,
		DoubleClick = 4,
		Enter = 8
	}
}