//  
//  trie.cs
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
	/// Date structure Trie.
	/// Reserved chars: '~' for root
	/// </summary>
	public class Trie
	{
		protected Node root;
		public Node Root { get { return root; } }
		protected Node end;
		public Node End { get { return end; } }
		protected ulong nodeCount;
		public ulong NodeCount {
			get { return nodeCount; }	
		}
		protected uint wordCount;
		public uint WordCount {
			get { return wordCount; }	
		}
		protected short maxDepth;
		public short MaxDepth {
			get { return maxDepth; }	
		}
		
		/* CONSTRUCTORS */
		
		/// <summary>
		/// Initializes a new instance of the <see cref="trie.Trie"/> class.
		/// </summary>
		public Trie ()
		{
			this.end = new Node('_');
			this.end.Finite = false;
			this.root = new Node('~');	
			this.nodeCount = 1;
			this.wordCount = 0;
			this.maxDepth = 0;
		}
		public Trie (string s) : this()	{ this.Add( s ); }
		public Trie (string[] sar) : this()	{ foreach( string s in sar ) this.Add(s); }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="trie.Trie"/> class.
		/// </summary>
		/// <param name='sr'>
		/// Sr - 
		/// </param>
		/// <param name='close'>
		/// Close. If true constructor close input stream.
		/// </param>
		public Trie(StreamReader sr, bool close = true) : this() {
			char c;
			List<char> tmp = new List<char>(6);
			while( ! sr.EndOfStream ) {
				c = (char) sr.Read();
				switch( c ) {
				case '\n' :
				case ',' : 
					continue;
				case ' ' :
					this.Add( new string( tmp.ToArray() ) );
					tmp.Clear();
					continue;
				default:
					tmp.Add( c );
					break;
				}
			}
			if( close ) sr.Close();
		}
		
		/* MAIN RUTINE FUNCTIONS */
		/// <summary>
		/// Test content of the s.
		/// </summary>
		/// <param name='s'>
		/// If set to <c>true</c> s.
		/// </param>
		public bool Content( string s ) {
			string s2 = s.ToUpperInvariant();
			Node tmp = root;
			
			for(int i=0; i<s2.Length; i++) {
				if( tmp.isSon( s2[i]) ) tmp = tmp.getSon( s2[i] );
				else return false;
			}
			return tmp.Finite;
		}
		
		public bool Content( char[,] de, int i, int j, bool down ) {
			Node tmp = root;
			
			while( de[i,j] != '_' ) {
				if( tmp.isSon( de[i,j] ) ) tmp = tmp.getSon( de[i,j] );
				else return false;
				
				if( down ) j++; else i++;
				
				if( i >= de.GetLength(0) ) return tmp.Finite;
				if( j >= de.GetLength(1) ) return tmp.Finite;
			}
			return tmp.Finite;	
		}
		
			public bool Content( char[,] de, int i, int j, bool down, int i2, int j2, char c ) {
			Node tmp = root;
			
			while( de[i,j] != '_' ) {
				if( tmp.isSon( de[i,j] ) ) tmp = tmp.getSon( de[i,j] );
				else return false;
				
				if( down ) j++; else i++;
				
				if( i == i2 && j == j2 ) {
					if( tmp.isSon( c ) ) tmp = tmp.getSon( c );
					else return false;	
					
					if( down ) j++; else i++;
				}
				if( i >= de.GetLength(0) ) return tmp.Finite;
				if( j >= de.GetLength(1) ) return tmp.Finite;
			}
			return tmp.Finite;	
		}
		
		public Node Find( string s ) {
			string s2 = s.ToUpperInvariant();
			
			Node tmp = root;
			for(int i=0; i<s2.Length; i++) {
				if( tmp.isSon( s2[i]) ) tmp = tmp.getSon( s2[i] );
				else return null;
			}
			return tmp;
		}
		
		public virtual void Add( string s  ) {
			if( s.Length == 0 ) return;
			string s2 = s.ToUpperInvariant();
			Node tmp = this.root;
			
			for(int i=0; i<s2.Length; i++)
				if( tmp.isSon( s2[i] ) ) {
					tmp = tmp.getSon( s2[i] );
				} else {
					tmp.addSon( s2[i] );
					this.nodeCount++;
					tmp = tmp.getSon( s2[i] );
				}
			
			tmp.Finite = true;
			
			if( s2.Length > maxDepth ) this.maxDepth = (short) s2.Length;
			this.wordCount++;
		}
		
		public virtual void Print() {
			root.print("");	
		}
	}
}

