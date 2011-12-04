//  
//  Player.cs
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
using System.Net;
using System.Collections.Generic;
using Scrabble.Game;
using Scrabble.Lexicon;

namespace Scrabble.Player
{
	[Serializable]
	public class Player
	{
		protected int id;
		public int ID { get {return id; } }
		protected string name;
		public string Name { get {return name;} }
		public int Score { get; set; }
		public List<char> Rack;
		
			
		[NonSerialized]
		protected Game.Game game;
		
		public Move bestMove = new Move("");
		
		public Player (string n)
		{
			this.name = n;
			this.Score = 0;
			this.Rack = new List<char>( Scrabble.Game.InitialConfig.sizeOfRack );
			this.id = Uniqe.GetFreeID();
		}
		
		public void ReloadRack() {
			Rack =  Scrabble.Game.StonesBag.ReloadAll( Rack );
			if( Scrabble.Game.InitialConfig.log ) 
				Scrabble.Game.InitialConfig.logStream.WriteLine("PLAYER {0} REALOAD RACK", this.Name);	
		}

		public override string ToString ()
		{
			return string.Format ("[Player: ID={0}, Name={1}, Score={2}]", ID, Name, Score);
		}
		
		public void SetGame( Game.Game g ) {
			this.game = g;	
		}
		
		public virtual bool DoMove( Lexicon.Move m ) {

			/* Check cross check 
			 * Calcul score */
			if( ! this.game.desk.AnalyzeMove( m ) ) return false;
			
			/* Is connected with rest of stone? */
			if( ! this.game.desk.Connect( m ) ) return false;
			
			
			
			// LOG
			if( Scrabble.Game.InitialConfig.log ) {
				Scrabble.Game.InitialConfig.logStream.Write("ROUND {1}\tPLAYER {0}\tRACK: ", this.Name, this.game.	Round);
				foreach( char c in this.Rack) {
					Scrabble.Game.InitialConfig.logStream.Write("{0} ", c);				
				}
				Scrabble.Game.InitialConfig.logStream.WriteLine();
			}
			
			if( ! this.game.desk.Play( m ) ) return false;	
			else { 
				if( m.Score > game.bestMove.Score ) game.bestMove = m;
				if( m.Score > this.bestMove.Score ) this.bestMove = m;
				WriteToLog( m );
			}
			return true;	
		}
		
		protected void WriteToLog(Move m) {
			if( Scrabble.Game.InitialConfig.log ) {
				Scrabble.Game.InitialConfig.logStream.Write("ROUND {5}\tPLAYER {0}\tSCORE +{4}\tAT {2:00},{3:00}\tPUT {1} : ", 
												this.Name, m.Word, m.Start.X, m.Start.Y, m.Score, this.game.Round);
				foreach( MovedStone ms in m.PutedStones) {
					Scrabble.Game.InitialConfig.logStream.Write(" [{1},{2}] -> {0} ;", ms.c, ms.i, ms.j);				
				}
				Scrabble.Game.InitialConfig.logStream.WriteLine();
			}	
		}
		
		public void Restart() {
			this.Score = 0;
			this.Rack.Clear();
		}
	}
	
	public static class Uniqe {
		private static int i = -1;
		
		public static int GetFreeID() {
			i++;
			return i;
		}
	}
	
	
	[Serializable]
	public class NetworkPlayer : Player {
		protected IPEndPoint ep;
		public IPAddress IP {get { return ep.Address; } }
		public IPEndPoint End {get { return ep; } }
		
		public NetworkPlayer(string n, string ipt) : base( n ) {
			IPAddress ip;
			if( ! IPAddress.TryParse( ipt,out ip ) ) {
				// TODO: Opakované zeptání se na IP adresu
				Environment.Exit(1);
			}
			this.ep = new IPEndPoint( ip, Scrabble.Game.InitialConfig.port );	
		}
		
		public override bool DoMove (Lexicon.Move m)
		{
			/* Check cross check 
			 * Calcul score */
			if( ! game.desk.AnalyzeMove( m ) ) return false;
			
			/* Is connected with rest of stone? */
			if( ! game.desk.Connect( m ) ) return false;
			
			lock( Scrabble.Game.InitialConfig.game.gameLock ) {
				Scrabble.Game.InitialConfig.game.Window.DisableButtons();
				Scrabble.Game.InitialConfig.game.move = m;
				Scrabble.Game.InitialConfig.game.turnDone = true;
				Scrabble.Game.InitialConfig.game.clientThread.Interrupt();
			}
			return true;
		}
	}
	
	
	/// <summary>
	/// Basic computer player (artificial inteligence), use greedy algorithm (czech: "hladový")
	/// </summary>
	[Serializable]
	public class ComputerPlayer : Player {
		protected AI ais;	
		
		public ComputerPlayer(string n, AI ai) : base ( n ) {
			this.ais = ai;	
		}
		
		public void Play() {
			// LOG
			if( Scrabble.Game.InitialConfig.log ) {
				Scrabble.Game.InitialConfig.logStream.Write("ROUND {1}\tPLAYER {0}\tRACK: ", this.Name, this.game.Round);
				foreach( char c in this.Rack) {
					Scrabble.Game.InitialConfig.logStream.Write("{0} ", c);				
				}
				Scrabble.Game.InitialConfig.logStream.WriteLine();
			}
			
			// SEARCH ALGORITHM
			HashSet<Move> movePool = new HashSet<Move>(); 
			for(int j=0; j < game.desk.Desk.GetLength(1); j++)
				for(int i=0; i < game.desk.Desk.GetLength(0); i++) {
					SearchAlgorithm.Search(i,j, this.Rack,movePool );
				}
			
			char[,] backUp = this.game.desk.Desk.DeepCopy();
			
			// ANALYZE AND CLEAN RESULTS
			HashSet<Move> toDel = new HashSet<Move>();
			
			// Next analyze
			Move max = new Move(new System.Drawing.Point(0,0),"",true);
			max.Score = -1;
	
			
			foreach( Move m in movePool ) {
				if( ! this.game.desk.AnalyzeMove( m ) )
					toDel.Add( m );
				if( m.PutedStones.Count == 0 )
					toDel.Add( m );
				else {
					if( max.Score < m.Score	)
						max = m;
				}
			}																																																																												
			foreach( Move m in toDel ) {
				movePool.Remove( m );	
			}
			
			if( backUp.SameValues( this.game.desk.Desk ) ) {}
			else {
#if DEBUG
				Scrabble.Game.InitialConfig.logStreamAI.WriteLine("[ERROR]\tNarušena deska (běheme analýzy v AI");
#endif
				Gtk.MessageDialog md = new Gtk.MessageDialog( 	this.game.Window, 
																Gtk.DialogFlags.Modal, 
																Gtk.MessageType.Error, 
																Gtk.ButtonsType.Ok, 
																false, 
								"Chybou programátora narušena deska! Nejspíš počítač nyní zahrál nesmyslný tah.");
				md.Run();
			}
							
#if DEBUG
			Scrabble.Game.InitialConfig.logStreamAI.Write( "\nROUND {2}\tPLAYER {1}\tNašel jsem {0} tahů:\t", movePool.Count, this.Name, this.game.Round );
			foreach( Move m in movePool ) {
				Scrabble.Game.InitialConfig.logStreamAI.Write("[{1},{2}]{0}{3} ", m.Word, m.Start.X, m.Start.Y, m.Down ? "↓" : "→");	
			}
			Scrabble.Game.InitialConfig.logStreamAI.WriteLine();
			Scrabble.Game.InitialConfig.logStreamAI.WriteLine("[info]\tVybral jsem: {0:00},{1:00} {2} {3}", max.Start.X, max.Start.Y, max.Word, max.Down ? "↓" : "→");
#endif

			if( movePool.Count == 0 ) {
				ReloadRack();
				return ;
			}
			
			foreach( Move ac in movePool ) {
				if( this.game.desk.Play( max ) ) break;
				max = ac;	
			}
						
			if( max.Score > game.bestMove.Score ) game.bestMove = max;
			if( max.Score > this.bestMove.Score ) this.bestMove = max;
			WriteToLog( max );
		}
		
	}
}

