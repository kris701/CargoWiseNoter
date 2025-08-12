using CargoWiseNoter.Models;
using CargoWiseNoter.Views;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;

namespace CargoWiseNoter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Window Helper Functions
		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
		{
			WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			GetWindowPlacement(hwnd, ref placement);
			return placement;
		}

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowPlacement(
			IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		internal struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public ShowWindowCommands showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}

		internal enum ShowWindowCommands : int
		{
			Hide = 0,
			Normal = 1,
			Minimized = 2,
			Maximized = 3,
		}

		public struct Rect
		{
			public int Left { get; set; }
			public int Top { get; set; }
			public int Right { get; set; }
			public int Bottom { get; set; }
		}
		#endregion		

		private DatabaseModel _data;
		private readonly int _delayMs = 250;
		private readonly string _dataName = "data.json";
		private nint _windowHandle = 0;
		private nint _lastWindowHandle = 0;
		private string _currentTitle = "";
		private NoteWindow? _currentNoteWindow;

		public MainWindow()
		{
			InitializeComponent();
			LoadData();

			Task.Run(async () =>
			{
				while (true)
					await DoProcess();
			});
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var wid = new System.Windows.Interop.WindowInteropHelper(this);
			_windowHandle = wid.Handle;
		}

		private async Task DoProcess()
		{
			// If no mstsc (remote desktop) process exists, wait for 10s
			if (!Process.GetProcesses().Any(x => x.ProcessName == "mstsc"))
			{
				await Task.Delay(10000);
				return;
			}

			// Check if the current foreground window is this one, if so then dont change anything
			var handle = GetForegroundWindow();
			if (handle == _windowHandle)
			{
				await Task.Delay(_delayMs);
				return;
			}

			// Check if the foreground window process is from mstsc
			// If not, hide this window and wait.
			uint processId = 0;
			GetWindowThreadProcessId(handle, out processId);
			var active = Process.GetProcessById((int)processId);
			if (active.ProcessName != "mstsc")
			{
				if (this.Visibility == Visibility.Visible)
				{
					this.Dispatcher.Invoke(() =>
					{
						this.Visibility = Visibility.Hidden;
					});
				}
				await Task.Delay(_delayMs * 4);
				return;
			}

			// Check if the foreground window is maximised or not.
			var isMaximised = GetPlacement(handle).showCmd == ShowWindowCommands.Maximized;
			if (_lastWindowHandle != handle && _currentNoteWindow != null)
				_currentNoteWindow.Dispatcher.Invoke(() => _currentNoteWindow.Close());
			_lastWindowHandle = handle;

			// Update this windows position based on the target foreground window.
			Rect windowRect = new Rect();
			GetWindowRect(GetForegroundWindow(), ref windowRect);
			await this.Dispatcher.InvokeAsync(() =>
			{
				var title = active.MainWindowTitle;
				_currentTitle = title.Substring(0, title.IndexOf('-'));
				if (this.Visibility == Visibility.Hidden)
					this.Visibility = Visibility.Visible;
				if (isMaximised)
				{
					this.Left = windowRect.Right - this.Width - 15;
					this.Top = windowRect.Bottom - this.Height - 15;
				}
				else
				{
					this.Left = windowRect.Right - this.Width / 2;
					this.Top = windowRect.Bottom - this.Height / 2;
				}
				if (_currentNoteWindow != null)
				{
					_currentNoteWindow.Left = this.Left - _currentNoteWindow.Width;
					_currentNoteWindow.Top = this.Top - _currentNoteWindow.Height;
				}
			});
			await Task.Delay(_delayMs);
		}

		private void LoadData()
		{
			if (!File.Exists(_dataName))
			{
				_data = new DatabaseModel();
				SaveData();
			}
			else
			{ 
				var notes = JsonSerializer.Deserialize<DatabaseModel>(File.ReadAllText(_dataName));
				if (notes == null)
					throw new Exception("Could not deserialize the NotesModel json file!");
				_data = notes;
			}
		}

		private void SaveData()
		{
			File.WriteAllText(_dataName, JsonSerializer.Serialize(_data));
		}

		private void LoadNote_Click(object sender, RoutedEventArgs e)
		{
			if (_currentNoteWindow != null)
				return;
			List<NoteModel> currentNotes;
			if (_data.Notes.ContainsKey(_currentTitle))
				currentNotes = _data.Notes[_currentTitle];
			else
				currentNotes = new List<NoteModel>() { new NoteModel() { Title = "New Note" } };

			_currentNoteWindow = new NoteWindow(_currentTitle, currentNotes, (n) =>
			{
				_data.Notes[n.ViewModel.NoteKey] = n.ViewModel.CurrentNotes;
				_data.NoteWindowWidth = n.Width;
				_data.NoteWindowHeight = n.Height;
				SaveData();
			}, _data.NoteWindowWidth, _data.NoteWindowHeight);
			_currentNoteWindow.Owner = this;
			_currentNoteWindow.Left = this.Left - _currentNoteWindow.Width;
			_currentNoteWindow.Top = this.Top - _currentNoteWindow.Height;
			_currentNoteWindow.ShowDialog();
			_currentNoteWindow = null;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SaveData();
			Close();
		}
	}
}