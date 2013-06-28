using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Photobooth
{
	public partial class KioskWindow
	{
		public string PromptText { get; set; }
		public string ImagePath { get; set; }
		public string ImageExtension { get; set; }
		public int DisplaySeconds { get; set; }

		private TextBlock promptTextblock;
		private FileSystemWatcher imageMonitor;

		private Queue<string> imageQueue = new Queue<string>();

		public KioskWindow(PhotoboothSettings settings)
		{
			PromptText = settings.PromptText;
			FontFamily = new FontFamily(settings.FontFamilyName);
			FontSize = settings.FontSize;
			ImagePath = settings.ImagePath;
			ImageExtension = settings.ImageExtension;
			DisplaySeconds = settings.DisplaySeconds;

			InitializeComponent();

			promptTextblock = new TextBlock
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				TextWrapping = TextWrapping.NoWrap,
				Text = PromptText
			};

			showPrompt();

			startMonitoring();
		}

		private void KioskWindow_OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.Close();
			}
		}

		private void startMonitoring()
		{
			try
			{
				imageMonitor = new FileSystemWatcher(ImagePath, ImageExtension);
				imageMonitor.Created += ImageMonitorOnCreated;
				imageMonitor.EnableRaisingEvents = true;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}

			Task queueWatcher = new Task(watchImageQueue);
			queueWatcher.Start();
		}

		private void watchImageQueue()
		{
			while (true)
			{
				while (imageQueue.Count > 0)
					displayQueueTop();

				showPrompt();

				Thread.Sleep(100);
			}
		}

		private void displayQueueTop()
		{
			string imagePath;
			lock (imageQueue)
			{
				imagePath = imageQueue.Dequeue();
			}

			showImage(imagePath);

			Thread.Sleep(DisplaySeconds * 1000);
		}

		private void ImageMonitorOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			lock (imageQueue)
			{
				imageQueue.Enqueue(fileSystemEventArgs.FullPath);
			}
		}

		private void showPrompt()
		{
			this.Dispatcher.Invoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				new Action(
					() =>
					{
						this.Content = promptTextblock;
					}));
		}

		private void showImage(string imagePath)
		{
			this.Dispatcher.Invoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				new Action(
					() =>
					{
						try
						{
							var bitmap = new BitmapImage();
							using (FileStream fs = new FileStream(imagePath, FileMode.Open))
							{
								bitmap.BeginInit();
								bitmap.CacheOption = BitmapCacheOption.OnLoad;
								bitmap.StreamSource = fs;
								bitmap.EndInit();
							}

							bitmap.Freeze();

							var photo = new Image
								{
									HorizontalAlignment = HorizontalAlignment.Center,
									VerticalAlignment = VerticalAlignment.Center,
									Source = bitmap
								};

							this.Content = photo;
						}
						catch (Exception)
						{
							// Probabably a threading exception, we can safely ignore, just keep chugging along
						}
					}));
		}
	}
}
