using CargoWiseNoter.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoWiseNoter.ViewModels
{
	public partial class NoteWindowViewModel : ObservableObject
	{
		[ObservableProperty]
		private string _noteKey;

		[ObservableProperty]
		private NoteModel _currentNote;

		public NoteWindowViewModel(string noteKey, NoteModel currentNote)
		{
			_noteKey = noteKey;
			_currentNote = currentNote;
		}
	}
}
