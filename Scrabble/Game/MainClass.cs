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
	/// <summary>
	/// Main class.
	/// </summary>
	public static class MainClass
	{
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main ( string[] args )
		{	
			#region UNIT TEST
			if ( Array.Exists<string>( args, (x) => x == "--test" ) ) {
				Scrabble.Testing.Tests.start();
				return;
			}
			#endregion
			
			Gtk.Application.Init( Environment.GetCommandLineArgs()[0] , ref args );
		
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
					#region END
					if( Scrabble.Game.InitialConfig.game.window.end ) {
						try {
							Scrabble.Game.InitialConfig.game.clientThread.Abort();
						} catch (NullReferenceException) { /* no network players */ }
						break;
					}
					#endregion
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
#if DEBUG
				if( Scrabble.Game.InitialConfig.logStreamAI != null )
					Scrabble.Game.InitialConfig.logStreamAI.Close();
#endif
			}
			#endregion
		}
	}
}

