using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoWiseNoter.Models
{
	public class DatabaseModel
	{
		public double NoteWindowWidth { get; set; } = 300;
		public double NoteWindowHeight { get; set; } = 500;
		public Dictionary<string, NoteModel> Notes { get; set; } = new Dictionary<string, NoteModel>();
	}
}
