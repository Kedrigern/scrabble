//  
//  Game.cs
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
using System.IO;

using Scrabble.Player;

namespace Scrabble.Game
{	
	public class Game
	{
		public const int MaxNumberOfPlayers = 4;
		public Scrabble.Lexicon.GADDAG dictionary;
		public Scrabble.Lexicon.PlayDesk desk;
		public Player.Player[] players;		
		public StonesBag stonesBag;
		private Scrabble.GUI.ScrabbleWindow window;
		public Scrabble.GUI.ScrabbleWindow Window {
			get {return window;}
		}
		int OnTurn = 0;
		
		public Game ()
		{	
			// Initialization of play desk (logic component, not gtk)
			desk = new Scrabble.Lexicon.PlayDesk ( this );
			// Initialization of bag for game stones
			stonesBag = new StonesBag();
			
			if( InitialConfig.gui ) {	
				Gtk.Application.Init ();
				
				// Window with game options
				GUI.StartGameWindow sw = new GUI.StartGameWindow( this );
				sw.Show();
				Gtk.Application.Run();
			}
		}
		
		public void PrepareDictionary(string path = "./dic.txt") {
			if( File.Exists( path ) ) {
				StreamReader sr = new StreamReader ( path );
				dictionary = new Scrabble.Lexicon.GADDAG( sr );	
			} else {
				dictionary = new Scrabble.Lexicon.GADDAG( 
						new string[] {"po", "do", "to", "lo", "dům", "doupě", "dole", "dolejší", "dort", "kolo", "molo", "na", "pa", "ba", "ma"}	// for testing purpose
					);
			}			
			// Init of search algorithm
			Scrabble.Lexicon.SearchAlgorithm.Init( desk.Desk , dictionary );	
		}
		
		public void SetPlayers( Scrabble.GUI.PlayerInit[] pls ) {
			players = new Scrabble.Player.Player [ pls.Length ];
			for(int i=0;i< pls.Length; i++) {
				players[i] = new Scrabble.Player.Player( pls[i].entry.Text, this );
			}
			//TODO: CPU
		}
		
		public Scrabble.Player.Player GetActualPlayer() {
			return players[ OnTurn ];	
		}
		
		public void IncActualPlayerScore(int s) {
			players[OnTurn].Score += s;	
		}
		
		public void changePlayer () {
			this.stonesBag.CompleteRack( ((Scrabble.Player.Player) players[OnTurn]).Rack );
#if DEBUG
			Console.WriteLine("Zásobník ({0})",((Scrabble.Player.Player) players[OnTurn]).Name );
			foreach(char c in ((Scrabble.Player.Player) players[OnTurn]).Rack )
				Console.Write("{0}, ", c);
			Console.WriteLine();
#endif
			OnTurn++;
			if( OnTurn >= players.Length ) OnTurn =0;
			Window.changePlayer( players[OnTurn] );
			
			GUI.StaticWindows.NextPlayer( players[OnTurn].Name );
		}
		
		/// <summary>
		/// Reloads the rack and change player - alternative way to turn.
		/// </summary>
		public void ReloadRackAndChange () {
			((Scrabble.Player.Player) players[OnTurn]).ReloadRack();
			changePlayer();
		}
		
		public void CreateMainWindowLoop() {
			window = new Scrabble.GUI.ScrabbleWindow( this );			
			Window.SetPosition( Gtk.WindowPosition.Center );
			
			// Inicialize dialogs from menu (like checkword, about etc.)
			Scrabble.GUI.StaticWindows.Init( this );
				
			//win.RackChange( ((Scrabble.Player.Player) players[0]).Rack );
			Window.changePlayer( players[OnTurn] );

			Window.Show ();
			Gtk.Application.Run ();
			Window.Destroy();	
			Window.Dispose();
		}
	}
}

