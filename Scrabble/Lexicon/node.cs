//  
//  node.cs
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

namespace Scrabble.Lexicon
{
	/// <summary>
	/// Node for <see cref="Trie"/> and <see cref="GADDAG"/>
	/// </summary>
	public class Node : IComparer<Node>, IComparer<Char>
	{
		private char content;
		
		/// <summary>
		/// Gets the node content.
		/// </summary>
		public char Content {
			get { return content; }	
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Scrabble.Lexicon.Node"/> is finite.
		/// </summary>
		/// <value>
		/// <c>true</c> if finite; otherwise, <c>false</c>.
		/// </value>
		public bool Finite {get; set;}
		List<Node> sons;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Scrabble.Lexicon.Node"/> class.
		/// </summary>
		public Node (char c, bool b = false)
		{
			this.content = c;
			this.Finite = b;
			sons = new List<Node>(2);
		}
		
		/// <summary>
		/// Is n2 son of this node?
		/// </summary>
		public bool isSon(Node n2) {
			return sons.Contains( n2 );	
		}
		/// <summary>
		/// Has this node son with c2?
		/// </summary>
		public bool isSon(char c2) {
			return sons.Exists( delegate ( Node n2	) { return n2.content == c2; } );
		}
		
		public Node getSon(char c) {
			return sons.Find( delegate ( Node n2 ) { return n2.content == c; } );
		}
		
		public void addSon(char c2) {
			sons.Add( new Node(c2) );	
		}
		
		public void addSubTree(Node n1) {
			if( sons.Exists( delegate ( Node n2	) { return n2.content == n1.content; } ) ) {
				throw new Exception("Error: Node allready exists.");
			} else sons.Add( n1 );	
		}
		
		public int CompareTo( Node n2) {
			if( content == n2.content ) return 0;	
			if( content < n2.content ) return -1;
			else return 1;
		}
		public int CompareTo( char c2) {
			if( content == c2 ) return 0;
			if( content < c2 ) return -1;
			else return 1;
		}
		public int Compare( Node n1, Node n2) {
			if( n1.content == n2.content ) return 0;	
			if( n1.content < n2.content ) return -1;
			else return 1;
		}
		public int Compare( char c1, char c2) {
			if( c1 == c2 ) return 0;	
			if( c1 < c2 ) return -1;
			else return 1;
		}
		
		public static bool operator<( Node n1, Node n2 ) {
			if( n1.CompareTo( n2 ) < 0 ) return true;
			else return false;
		}
		public static bool operator>(Node n1, Node n2) {
			if( n1.CompareTo( n2 ) > 0 ) return true;
			else return false;
		}		

		public void print(string s, StreamWriter sw) {
			string sn = s + (content == '~' ? "" : this.content.ToString() );
			if( this.Finite ) sw.Write( "{0} ", sn );

			foreach( Node n in sons ) {
				n.print( sn , sw );	
			}	
		}
	}
}