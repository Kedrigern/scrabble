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
using System.Threading;
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
		
		// Statistic data
		public Scrabble.Lexicon.Move bestMove = new Scrabble.Lexicon.Move("");
		
		// History of game;
		public Stack<Scrabble.Lexicon.Move> historyM;
		public Stack<Scrabble.Lexicon.Move> futureM;
		
		public bool newData = false;
		
		int OnTurn = 0;
		int round = 1;
		bool morePeople = false;
		bool networkPlayers = false;
		bool client = false;
	
		// server specific
		scrabbleServer sserver = null;
		
		// client specific
		public bool yourTurn = false;
		public bool turnDone = false;
		public Lexicon.Move move = null;
		Scrabble.Game.scrabbleClient sclient = null;
		public Thread clientThread = null;
		public object gameLock = new object();
		public NetworkCarrierPlayer ncp = null;
		
		public Game(bool isClient = false ) {
			if( Scrabble.Game.InitialConfig.dictionary == null )
				throw new NullReferenceException("During game initialization is Scrabble.Game.InitialConfig.dictionary == null");
			else 
				this.dictionary	= Scrabble.Game.InitialConfig.dictionary;
		
			this.client = isClient;
			this.round = 1;
			this.historyM = new Stack<Scrabble.Lexicon.Move>(20);
			this.futureM = new Stack<Scrabble.Lexicon.Move>(6);
			
			// Initialization of play desk (logic component, not gtk)
			this.desk = new Scrabble.Lexicon.PlayDesk ( this );
			// Initialization of bag for game stones
			this.stonesBag = new StonesBag();
			
			global::Scrabble.Lexicon.SearchAlgorithm.Init( desk.Desk, this.dictionary );
			
			this.players = Scrabble.Game.InitialConfig.players;
			
			if( isClient ) {
				this.morePeople = true;
			} else {
				sserver = new scrabbleServer( this );
				int k =0;
				int l =0; 
				foreach( Scrabble.Player.Player p in players ) {
					if( p.GetType() == typeof( Scrabble.Player.Player ) ) k++;
					if( p.GetType() == typeof( Scrabble.Player.NetworkPlayer ) ) l++;
					p.SetGame( this );
					p.ReloadRack();
				}
				if( k > 1 ) this.morePeople = true;
				if( l > 0 ) this.networkPlayers = true;
			}
			
			// Inicialize dialogs from menu (like checkword, about etc.)
			Scrabble.GUI.StaticWindows.Init( this );
			
			if( this.client ) {
				this.sclient = new scrabbleClient( this );
				this.clientThread = new Thread( this.mainClientLoop );
				this.clientThread.Start();
			} else {
				this.sendUpdatViaNetwork( true );
			}
		}
		
		#region BASIC LOGIC
		public Scrabble.Player.Player GetActualPlayer() {
			return players[ OnTurn ];	
		}
		
		public void IncActualPlayerScore(int s) {
			players[OnTurn].Score += s;	
		}	
		
		/// <summary>
		/// Reloads the rack and change player - alternative way to turn.
		/// </summary>
		public void ReloadRackAndChange () {
			((Scrabble.Player.Player) players[OnTurn]).ReloadRack();
			changePlayer();
		}
		
		private void nextPlayerIndex() {
			this.OnTurn++;
			if( this.OnTurn >= this.players.Length ) {
				this.OnTurn =0;
				this.round++;
				Scrabble.Game.InitialConfig.logStream.Flush();
#if DEBUG
				Scrabble.Game.InitialConfig.logStreamAI.Flush();
#endif
			}
		}
		#endregion		
		
		#region MAIN LOOP and SERVER
		public void changePlayer () {
			this.stonesBag.CompleteRack( ((Scrabble.Player.Player) players[OnTurn]).Rack );
			
			this.nextPlayerIndex();

			this.Window.changePlayer( players[OnTurn] );
			
			if( this.networkPlayers ) this.sendUpdatViaNetwork( false );
			
			if( typeof( ComputerPlayer ) == players[ OnTurn ].GetType() ) {
				this.Window.DisableButtons();	
				((ComputerPlayer) players[OnTurn]).Play();
				this.Window.ActiveButtons();
				this.changePlayer();
			} else if( typeof( NetworkPlayer ) == players[ OnTurn ].GetType() ) {
				this.Window.DisableButtons();	
				this.sserver.sendQuestion( ((NetworkPlayer) players[ OnTurn]).IP.ToString() );
				this.Window.ActiveButtons();
				this.changePlayer();
			} else if( this.morePeople )				
				GUI.StaticWindows.NextPlayer( players[OnTurn].Name );
		}
		
		private void sendUpdatViaNetwork(bool full = false) {
			foreach( Scrabble.Player.Player p in players ) {
				if( p.GetType() == typeof( NetworkPlayer ) ) {
					if( full )
						sserver.sendFullInfo( ((NetworkPlayer) p).IP.ToString() );
					else
						sserver.sendMiniInfo( ((NetworkPlayer) p).IP.ToString() );
				}
			}
		}
		
		public NetworkCarrierPlayer getNCP() {
			
			return new NetworkCarrierPlayer( OnTurn, players[OnTurn].Rack );	
		}
		#endregion
		

		#region GAME CONTROL
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
			
			this.window.Update();
			this.window.ShowAll();
		}
	
		public void back() {
			Scrabble.Lexicon.Move tmp = historyM.Pop();
			futureM.Push( tmp );
			foreach( Scrabble.Lexicon.MovedStone m in tmp.PutedStones)
				desk.Desk[ m.i , m.j ] = '_';
			
		}
		
		/// <summary>
		/// TODO
		/// </summary>
		public void forward() {
			
		}
		#endregion
		
		#region CLIENT
		/// <summary>
		/// Update data in this class. Use client!
		/// </summary>
		public void networkUpdate( NetworkCarrierMini c ) {				
			lock( this.gameLock ) {
				this.newData = true;
				for(int i=0; i<this.players.Length;i++) {
					this.players[i].Score = c.scores[i];	
				}
				this.desk.Desk = c.desk;
			}
		}
		
		public void networkUpdate( NetworkCarrierFull c ) {				
			lock( this.gameLock ) {
				this.newData = true;
				this.players = c.players;
				foreach( Scrabble.Player.Player p in this.players ) 
					p.SetGame( this );
				this.desk = c.playDesk;
				this.desk.setGame( this );
			}
		}
		
		public void mainClientLoop() {
			this.sclient.mainLoop();
		}
		
		public void clientTurn() {
			this.yourTurn = false;
			this.players[ ncp.order ].Rack = ncp.rack;
			this.OnTurn = ncp.order;
			this.Window.changePlayer( this.players[ ncp.order] );
			this.Window.ActiveButtons();
			
		}
		#endregion
	}
}

