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
using System.Collections.Generic;

using Scrabble.Player;

namespace Scrabble.Game
{	
	public class Game
	{
		public const int MaxNumberOfPlayers = 4;
		public int Round { get { return round; } }
		public Scrabble.Lexicon.GADDAG dictionary;
		public Scrabble.Lexicon.PlayDesk desk;
		public Player.Player[] players;		
		public StonesBag stonesBag;
		public Scrabble.GUI.ScrabbleWindow window;
		public Scrabble.GUI.ScrabbleWindow Window {
			get {return window;}
		}
		
		public Scrabble.Lexicon.Move bestMove = new Scrabble.Lexicon.Move("");
		
		public Stack<Scrabble.Lexicon.Move> history;
		public Stack<Scrabble.Lexicon.Move> future;
		
		int OnTurn = 0;
		int round;
	
		public Game( ) {
			if( Scrabble.Game.InitialConfig.dictionary == null )
				throw new NullReferenceException("During game initialization is Scrabble.Game.InitialConfig.dictionary == null");
			else 
				this.dictionary	= Scrabble.Game.InitialConfig.dictionary;
		
			this.round = 1;
			this.history = new Stack<Scrabble.Lexicon.Move>(20);
			this.future = new Stack<Scrabble.Lexicon.Move>(6);
			
			// Initialization of play desk (logic component, not gtk)
			desk = new Scrabble.Lexicon.PlayDesk ( this );
			// Initialization of bag for game stones
			stonesBag = new StonesBag();
			
			Lexicon.SearchAlgorithm.Init( desk.Desk, this.dictionary );
			
			this.players = Scrabble.Game.InitialConfig.players;
			foreach( Scrabble.Player.Player p in players ) {
				p.SetGame( this );
				p.ReloadRack();
			}
			
			// Inicialize dialogs from menu (like checkword, about etc.)
			Scrabble.GUI.StaticWindows.Init( this );
		}
		
		public Scrabble.Player.Player GetActualPlayer() {
			return players[ OnTurn ];	
		}
		
		public void IncActualPlayerScore(int s) {
			players[OnTurn].Score += s;	
		}
		
		public void changePlayer () {
			this.stonesBag.CompleteRack( ((Scrabble.Player.Player) players[OnTurn]).Rack );

			OnTurn++;
			if( OnTurn >= players.Length ) {
				OnTurn =0;
				round++;
				Scrabble.Game.InitialConfig.logStream.Flush();
#if DEBUG
				Scrabble.Game.InitialConfig.logStreamAI.Flush();
#endif
			}
			Window.changePlayer( players[OnTurn] );
			
			if( typeof( ComputerPlayer ) == players[ OnTurn ].GetType() ) {
				window.DisableButtons();	
				((ComputerPlayer) players[OnTurn]).Play();
				window.ActiveButtons();
				changePlayer();
			} else 		
				GUI.StaticWindows.NextPlayer( players[OnTurn].Name );
		}
		
		/// <summary>
		/// Reloads the rack and change player - alternative way to turn.
		/// </summary>
		public void ReloadRackAndChange () {
			((Scrabble.Player.Player) players[OnTurn]).ReloadRack();
			changePlayer();
		}
		
		public void newGame() {
			this.window.Hide();
			this.round = 1;
			this.desk = new Scrabble.Lexicon.PlayDesk(this);
			this.stonesBag = new StonesBag();
			foreach( Scrabble.Player.Player p in this.players ) {
				p.Restart();	
				this.stonesBag.CompleteRack( p.Rack );
			}
			OnTurn = 0;
			
			this.bestMove = new Scrabble.Lexicon.Move("");
			
			this.window.Restart();
			this.window.ShowAll();
		}
		
		public void back() {
			Scrabble.Lexicon.Move tmp = history.Pop();
			future.Push( tmp );
			
		}
		
		public void forward() {
			
		}
	}
}

