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
	public class Player
	{
		protected int id;
		public int ID { get {return id; } }
		protected string name;
		public string Name { get {return name;} }
		public int Score { get; set; }
		public List<char> Rack;
		protected Game.Game game;
		
		public Player (string n)
		{
			this.name = n;
			this.Score = 0;
			Rack = new List<char>( Scrabble.Game.InitialConfig.sizeOfRack );
			this.id = Uniqe.GetFreeID();
		}
		
		public void ReloadRack() {
			Rack = game.stonesBag.ReloadAll( Rack );
		}

		public override string ToString ()
		{
			return string.Format ("[Player: ID={0}, Name={1}, Score={2}]", ID, Name, Score);
		}
		
		public void SetGame( Game.Game g ) {
			this.game = g;	
		}
		
		public bool DoMove( Lexicon.Move m ) {
#if DEBUG
			Console.WriteLine("[info]\tChci položit {0} na [{1},{2}]", m.Word, m.Start.X, m.Start.Y);
#endif
			/* Check cross check 
			 * Calcul score */
			if( ! game.desk.AnalyzeMove( m ) ) return false;
			
			/* Is connected with rest of stone? */
			if( ! game.desk.Connect( m ) ) {
#if DEBUG
				Console.WriteLine( "[NO] \tŠpatné napojení" );
#endif
				return false; 
			}
			
			if( ! game.desk.Play( m ) ) return false;	
			else return true;	
		}
	}
	
	public static class Uniqe {
		private static int i = -1;
		
		public static int GetFreeID() {
			i++;
			return i;
		}
	}
	
	
	public class NetworkPlayer : Player {
		protected IPEndPoint ep;
		
		public NetworkPlayer(string n, string ipt) : base( n ) {
			IPAddress ip;
			if( ! IPAddress.TryParse( ipt,out ip ) ) {
				// TODO: Opakované zeptání se na IP adresu
				Console.WriteLine("[ERROR] Parsing IP adress");
				Environment.Exit(1);
			}
			this.ep = new IPEndPoint( ip, Scrabble.Game.InitialConfig.port );	
		}
	}
	
	/// <summary>
	/// Basic computer player (artificial inteligence), use greedy algorithm (czech: "hladový")
	/// </summary>
	public class ComputerPlayer : Player {
		protected AI ais;	
		
		public ComputerPlayer(string n, AI ai) : base ( n ) {
			this.ais = ai;	
		}
		
		public void Play() {
			HashSet<Move> movePool = new HashSet<Move>(); 
			for(int j=0; j < game.desk.Desk.GetLength(1); j++)
				for(int i=0; i < game.desk.Desk.GetLength(0); i++) {
					SearchAlgorithm.Search(i,j, this.Rack,movePool );
				}
			
			HashSet<Move> toDel = new HashSet<Move>();
			Move max = new Move(new System.Drawing.Point(0,0),"",true);
			max.Score = -1;
			foreach( Move m in movePool ) {
				game.desk.AnalyzeMove( m );
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
							
#if DEBUG
			Console.Write( "[info]\tMůj zasobník:");
			foreach( char c in Rack ) {
				Console.Write("{0} ", c);	
			}
			Console.Write( "\n[info]\tNašel jsem {0} tahů:\t", movePool.Count );
			foreach( Move m in movePool ) {
				Console.Write("[{1},{2}]{0}(3) ", m.Word, m.Start.X, m.Start.Y, m.Score);	
			}
			Console.WriteLine();
			Console.WriteLine("[info]\tIdeáal:[{1},{2}]{0}(3) ", max.Word, max.Start.X, max.Start.Y, max.Score);
#endif
			if( movePool.Count == 0 ) {
				this.ReloadRack();
				return ;
			}
				
			game.desk.Play( max );																																																																																																																																																																																																																																																																								game.desk.Play( max );
			
		}
	}
}

