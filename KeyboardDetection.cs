﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImagePasteApp
{
	// Code by rvknth043
	// Available here
	// https://github.com/rvknth043/Global-Low-Level-Key-Board-And-Mouse-Hook

	/// <summary>
	/// Class for intercepting low level keyboard hooks
	/// </summary>
	public class KeyboardDetection
	{
		/// <summary>
		/// Virtual Keys
		/// </summary>
		public enum VKeys
		{
			CONTROL = 0x11,     // CTRL key
			KEY_V = 0x56,       // V key
			SCAN_CONTROL = 162  // Left CTRL key
		}

		/// <summary>
		/// Internal callback processing function
		/// </summary>
		private delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
		private KeyboardHookHandler hookHandler;

		/// <summary>
		/// Function that will be called when defined events occur
		/// </summary>
		/// <param name="key">VKeys</param>
		public delegate void KeyboardHookCallback(VKeys key);

		#region Events
		public event KeyboardHookCallback KeyDown;
		public event KeyboardHookCallback KeyUp;
		#endregion

		/// <summary>
		/// Hook ID
		/// </summary>
		private IntPtr hookID = IntPtr.Zero;

		/// <summary>
		/// Install low level keyboard hook
		/// </summary>
		public void Install()
		{
			hookHandler = HookFunc;
			hookID = SetHook(hookHandler);
		}

		/// <summary>
		/// Remove low level keyboard hook
		/// </summary>
		public void Uninstall()
		{
			UnhookWindowsHookEx(hookID);
		}

		/// <summary>
		/// Registers hook with Windows API
		/// </summary>
		/// <param name="proc">Callback function</param>
		/// <returns>Hook ID</returns>
		private IntPtr SetHook(KeyboardHookHandler proc)
		{
			using (ProcessModule module = Process.GetCurrentProcess().MainModule)
				return SetWindowsHookEx(13, proc, GetModuleHandle(module.ModuleName), 0);
		}

		/// <summary>
		/// Default hook call, which analyses pressed keys
		/// </summary>
		private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0)
			{
				int iwParam = wParam.ToInt32();

				if ((iwParam == WM_KEYDOWN || iwParam == WM_SYSKEYDOWN))
					if (KeyDown != null)
						KeyDown((VKeys)Marshal.ReadInt32(lParam));
				if ((iwParam == WM_KEYUP || iwParam == WM_SYSKEYUP))
					if (KeyUp != null)
						KeyUp((VKeys)Marshal.ReadInt32(lParam));
			}

			return CallNextHookEx(hookID, nCode, wParam, lParam);
		}

		/// <summary>
		/// Destructor. Unhook current hook
		/// </summary>
		~KeyboardDetection()
		{
			Uninstall();
		}

		/// <summary>
		/// Low-Level function declarations
		/// </summary>
		#region WinAPI
		private const int WM_KEYDOWN = 0x100;
		private const int WM_SYSKEYDOWN = 0x104;
		private const int WM_KEYUP = 0x101;
		private const int WM_SYSKEYUP = 0x105;

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);
		#endregion
	}
}