//  
//  move.cs
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
using System.Drawing;

namespace Scrabble.Lexicon
{	
	/// <summary>
	/// Move ready to put at game desk.
	/// </summary>
	[Serializable]
	public class Move
	{
		private Point start;
		public Point Start {
			get { return start; }	
		}
		private bool down;
		public bool Down {
			get {return down; }	
		}
		public string Word;
		List<MovedStone> putedStones;
		public List<MovedStone> PutedStones {
			get { return putedStones; }
		}
		int score;
		public int Score {
			get { return score; }	
			set { score = value; }
		}
		
		public Move (Point start2, string word2, bool down2 = false)
		{
			this.start = start2;
			this.Word = word2;
			this.score = 0;
			this.down = down2;
			this.putedStones = new List<MovedStone>(4);
		}
		

		public override string ToString ()
		{
			return string.Format ("Tah: na [{0},{1}] {2} {3}", start.X, Start.Y, Word, Down ? "↓" : "→");
		}
		
		public Move( string s ) {
			this.Word = s;	
		}
		
		public void AddLetterToPut( MovedStone m ) {
			putedStones.Add( m );	
		}
	}
	
	enum Direction {
		left, right, down, up
	}
	
	/// <summary>
	/// Struct that identify conrete position for one new stone. 
	/// List<MovedStone> uniqe define all move
	/// </summary>
	public struct MovedStone {
		public char c;	
		public int i;
		public int j;
		
		public MovedStone(char cc, int ii, int jj) {
			this.c = cc;
			this.i = ii;
			this.j = jj;
		}
		
		public MovedStone(char cc, System.Drawing.Point p) {
			this.c = cc;
			this.i = p.X;
			this.j = p.Y;
		}
	}
}
