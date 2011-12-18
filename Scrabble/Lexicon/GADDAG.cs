//  
//  GADDAG.cs
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
	/// GADDAG is data structure derived from Trie, optimalized for Scrabble.
	/// </summary>
	public sealed class GADDAG : Trie
	{
		private Node reverse;
		/// <summary>
		/// Gets the reverse root (start of reverted word)
		/// </summary>
		/// <value>
		/// The reverse root.
		/// </value>
		public Node ReverseRoot { get { return reverse;} }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Scrabble.Lexicon.GADDAG"/> class.
		/// </summary>
		public GADDAG ()
		{
			this.root = new Node('~');	
			this.reverse = new Node('<');	
			this.nodeCount = 2;
			this.wordCount = 0;
			this.maxDepth = 0;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Scrabble.Lexicon.GADDAG"/> class.
		/// </summary>
		/// <param name='s'>
		/// String s is added to GADDAG
		/// </param>
		public GADDAG ( string s ) : this() { this.Add(s); }
		/// <summary>
		/// Initializes a new instance of the <see cref="Scrabble.Lexicon.GADDAG"/> class.
		/// </summary>
		/// <param name='sar'>
		/// Strings for sar[] are added to GADDAG
		/// </param>
		public GADDAG ( string[] sar ) : this() { foreach(string s in sar) this.Add(s); }
		/// <summary>
		/// Initializes a new instance of the <see cref="Scrabble.Lexicon.GADDAG"/> class.
		/// </summary>
		/// <param name='sr'>
		/// Words in proper format are added. Format: word, word2, word3,...
		/// </param>
		/// <param name='close'>
		/// Close the stream after?
		/// </param>
		public GADDAG ( StreamReader sr, bool close = true ) : this() { 
			/* Copy of implementation this constructor in trie, there is call GADDAG ADD */
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
		
		/// <summary>
		/// Add the specified string s to GADDAG dictionary structure.
		/// </summary>
		/// <param name='s'>
		/// S.
		/// </param>
		public override void Add (string s)
		{
			string s2 = s.ToUpperInvariant();
			if( s.Length == 0 ) return;
			base.Add (s2);		// Add in origin trie
			
			string prefix;
			for(int i=1; i<s2.Length; i++) {
				prefix = s2.Substring(0,i);
				char[] ch = prefix.ToCharArray();
				Array.Reverse( ch );
				this.AddPref( new string (ch) + ">" + s2.Substring(i) );
			}				
			char[] ch2 = s2.ToCharArray();
			Array.Reverse(ch2);
			this.AddPref( new string( ch2 ) + '>' );
		}
		
		private void AddPref( string s ) {
			Node tmp = this.reverse;
			
			for(int i=0; i<s.Length; i++)
				if( tmp.isSon( s[i] ) ) {
					tmp = tmp.getSon( s[i] );
				} else {
					tmp.addSon( s[i] );
					this.nodeCount++;
					tmp = tmp.getSon( s[i] );
				}
			
			tmp.Finite = true;
		}	
	}
}