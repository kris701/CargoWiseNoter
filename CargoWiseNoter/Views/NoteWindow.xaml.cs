using CargoWiseNoter.Models;
using CargoWiseNoter.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CargoWiseNoter.Views
{
	/// <summary>
	/// Interaction logic for NoteWindow.xaml
	/// </summary>
	public partial class NoteWindow : Window
	{
		public NoteWindowViewModel ViewModel { get; set; }
		private readonly Action<NoteWindow> _doSave;

		public NoteWindow(string noteKey, NoteModel note, Action<NoteWindow> doSave, double width, double height)
		{
			_doSave = doSave;
			DataContext = this;
			ViewModel = new NoteWindowViewModel(noteKey, note);
			InitializeComponent();
			Width = width;
			Height = height;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => _doSave.Invoke(this);

		private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
	}
}
