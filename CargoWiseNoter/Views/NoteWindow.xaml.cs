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

		public NoteWindow(string noteKey, List<NoteModel> notes, Action<NoteWindow> doSave, double width, double height)
		{
			_doSave = doSave;
			DataContext = this;
			ViewModel = new NoteWindowViewModel(noteKey, notes);
			InitializeComponent();
			Width = width;
			Height = height;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => _doSave.Invoke(this);

		private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

		private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.CurrentNotes.Remove(ViewModel.CurrentNote);
			if (ViewModel.CurrentNotes.Count == 0)
				ViewModel.CurrentNotes.Add(new NoteModel() { Title = "New Note" });
			ViewModel.CurrentNotes = new List<NoteModel>(ViewModel.CurrentNotes);
			ViewModel.CurrentNote = ViewModel.CurrentNotes[0];
		}

		private void AddNoteButton_Click(object sender, RoutedEventArgs e)
		{
			var newNote = new NoteModel() { Title = "New Note" };
			ViewModel.CurrentNotes.Add(newNote);
			ViewModel.CurrentNotes = new List<NoteModel>(ViewModel.CurrentNotes);
			ViewModel.CurrentNote = newNote;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ViewModel.CurrentNotes = new List<NoteModel>(ViewModel.CurrentNotes);
		}
	}
}
