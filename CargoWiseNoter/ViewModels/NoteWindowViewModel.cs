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
		private List<NoteModel> _currentNotes;

		[ObservableProperty]
		private NoteModel _currentNote;

		public NoteWindowViewModel(string noteKey, List<NoteModel> currentNotes)
		{
			_noteKey = noteKey;
			_currentNotes = currentNotes;
			_currentNote = currentNotes[0];
		}
	}
}
