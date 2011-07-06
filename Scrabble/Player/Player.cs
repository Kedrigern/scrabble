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
				// TODO: Opakované zeptání se na adresu
				Console.WriteLine("[ERROR] Parsong IP adress");
				Environment.Exit(1);
			}
			this.ep = new IPEndPoint( ip, Scrabble.Game.InitialConfig.port );	
		}
	}
	
	public class ComputerPlayer : Player {
		protected object AI;	
		
		public ComputerPlayer(string n, object ai) : base ( n ) {
			this.AI = ai;	
		}
	}
}

