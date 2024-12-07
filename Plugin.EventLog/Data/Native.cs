using System;
using System.Runtime.InteropServices;

namespace Plugin.EventLog.Data
{
	internal static class Native
	{
		public const Int32 LVM_GETHEADER = 0x1000 + 31; //	LVM_FIRST + 31

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public Int32 Left;	// x position of upper-left corner
			public Int32 Top;		// y position of upper-left corner
			public Int32 Right;	// x position of lower-right corner
			public Int32 Bottom;	// y position of lower-right corner
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean GetWindowRect(HandleRef hwnd, out RECT lpRect);
	}
}