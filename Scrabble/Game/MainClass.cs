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
			
			#region INIT WINDOW
			var startwin = new Scrabble.GUI.StartWindow();
			startwin.Show();
			Gtk.Application.Run();
			#endregion
			
			#region MAIN WINDOW
			try {
				Scrabble.Game.InitialConfig.game.window = new Scrabble.GUI.ScrabbleWindow( Scrabble.Game.InitialConfig.client );
				Scrabble.Game.InitialConfig.game.window.ShowAll();
				while( true ) {
					Gtk.Application.RunIteration();
					lock(  Scrabble.Game.InitialConfig.game.gameLock ) {
						if( Scrabble.Game.InitialConfig.game.newData ) {
							Scrabble.Game.InitialConfig.game.newData = false;
							Scrabble.Game.InitialConfig.game.Window.Update();
						}
					}
					lock( Scrabble.Game.InitialConfig.game.gameLock ) {
						if(	Scrabble.Game.InitialConfig.game.yourTurn ) {
							Scrabble.Game.InitialConfig.game.clientTurn();
						}
					}
					if( Scrabble.Game.InitialConfig.game.window.end )
						break;
				}
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
				if( Scrabble.Game.InitialConfig.logStreamAI != null )
					Scrabble.Game.InitialConfig.logStreamAI.Close();
#endif
			}
			#endregion
		}
	}
}

