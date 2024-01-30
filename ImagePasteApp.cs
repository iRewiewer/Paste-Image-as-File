namespace ImagePasteApp
{
	class ImagePasteApp : ApplicationContext
	{
		private NotifyIcon trayIcon;

		public ImagePasteApp()
		{
			InitializeTrayIcon();
		}

		private void InitializeTrayIcon()
		{
			trayIcon = new NotifyIcon()
			{
				Icon = SystemIcons.Application,
				ContextMenuStrip = new ContextMenuStrip(),
				Visible = true
			};

			trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, Exit));
			ClipboardWatcher.OnClipboardChange += OnClipboardChange;
		}

		private void OnClipboardChange(object sender, EventArgs e)
		{
			if (Clipboard.ContainsImage())
			{
				// Get the image from the clipboard
				Image image = Clipboard.GetImage();

				// Specify the folder where you want to save the image
				string folderPath = "YourFolderPathHere";

				// Generate a unique file name or handle duplicates
				string fileName = Guid.NewGuid().ToString() + ".png";

				// Save the image to the specified folder
				string filePath = Path.Combine(folderPath, fileName);
				image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
			}
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

	public static class ClipboardWatcher
	{
		public static event EventHandler OnClipboardChange;

		public static void StartWatching()
		{
			ClipboardWatcherForm form = new ClipboardWatcherForm();
			Application.Run(form);
		}

		private class ClipboardWatcherForm : Form
		{
			protected override void WndProc(ref Message m)
			{
				const int WM_CLIPBOARDUPDATE = 0x031D;

				if (m.Msg == WM_CLIPBOARDUPDATE)
				{
					OnClipboardChange?.Invoke(this, EventArgs.Empty);
				}

				base.WndProc(ref m);
			}

			protected override void SetVisibleCore(bool value)
			{
				base.SetVisibleCore(false);
			}
		}
	}
}