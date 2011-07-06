using System;
using Gtk;

namespace Scrabble.Game
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			var win = new Scrabble.GUI.ScrabbleWindow();
			win.Show ();
			Application.Run ();
		}
	}
}
