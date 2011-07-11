//  
//  MainClass.cs
//  
//  Author:
//       Ondřej Profant <ondrej.profant@gmail.com>
// 
//  Copyright (c) 2011 Ondřej Profant
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Gtk;

namespace Scrabble.Game
{
	public static class MainClass
	{
		public static void Main ()
		{
			Gtk.Application.Init();
			
			var startwin = new Scrabble.GUI.StartWindow();
			startwin.Show();
			Gtk.Application.Run();
			
			/* ! ! ! BUG ! ! !
			 * Při spuštění spuštění jinak než s MonoDevelop či terminalu se neotevře druhé okno.
			 * Avšak chyba je někde v něm, jelikož pokud zkopirujeme kod nastavovacího okna pod něj, 
			 * tak to se otevře normálně dvakrát
			 * */
			try {
				Scrabble.Game.InitialConfig.game.window = new Scrabble.GUI.ScrabbleWindow();
				Scrabble.Game.InitialConfig.game.window.ShowAll();
				Gtk.Application.Run();
			} catch (Exception e) {
				Gtk.MessageDialog md = new Gtk.MessageDialog( 
					null, 
					DialogFlags.Modal,
					MessageType.Error,
					ButtonsType.Ok, 
					e.Message );	
				md.Run();				
			} finally {
				Scrabble.Game.InitialConfig.logStream.Flush();
				Scrabble.Game.InitialConfig.logStream.Close();
#if DEBUG
				Scrabble.Game.InitialConfig.logStreamAI.Close();
#endif
			}
		}
	}
}

