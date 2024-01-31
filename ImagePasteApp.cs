namespace ImagePasteApp
{
	class ImagePasteApp : ApplicationContext
	{
		private NotifyIcon trayIcon;
		private KeyboardDetection keyboardHook;

		public ImagePasteApp()
		{
			InitializeTrayIcon();
			InitializeKeyboardHook();
		}

		private void InitializeTrayIcon()
		{
			trayIcon = new NotifyIcon()
			{
				Icon = SystemIcons.Application,
				Visible = true
			};

			trayIcon.ContextMenuStrip = new ContextMenuStrip();
			trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, Exit));
		}

		private void InitializeKeyboardHook()
		{
			keyboardHook = new KeyboardDetection();
			keyboardHook.KeyDown += KeyboardHook_KeyDown;
			keyboardHook.Install();
		}

		private void KeyboardHook_KeyDown(KeyboardDetection.VKeys key)
		{
			// Check for CTRL + V
			if (key == KeyboardDetection.VKeys.CONTROL || key == KeyboardDetection.VKeys.SCAN_CONTROL)
			{
				// Handle CTRL key pressed, check if V is pressed too
				keyboardHook.KeyUp += KeyboardHook_KeyUpForCtrlV;
			}
		}

		private void KeyboardHook_KeyUpForCtrlV(KeyboardDetection.VKeys key)
		{
			// Check for CTRL + V
			if (key == KeyboardDetection.VKeys.KEY_V)
			{
				// Handle CTRL + V pressed

				// Implement logic to check if the focused window is Windows Explorer and clipboard contains an image
				// If conditions are met, create a new .png image file in the corresponding folder
			}

			// Unsubscribe to prevent unnecessary processing for other keys
			keyboardHook.KeyUp -= KeyboardHook_KeyUpForCtrlV;
		}

		private void Exit(object sender, EventArgs e)
		{
			trayIcon.Visible = false;
			Application.Exit();
		}

		[STAThread]
		static void Main()
		{
			Application.Run(new ImagePasteApp());
		}
	}
}