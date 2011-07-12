//  
//  desk.cs
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
	public class PlayDesk
	{	
		char[,] desk;
		byte[,] wordBonus;
		byte[,] charBonus;
		
		public char[,] Desk { get { return desk;} }
		public byte[,] WordBonus { get { return wordBonus;} }
		public byte[,] CharBonus { get { return charBonus;} }
		
		Scrabble.Game.Game game;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="trie.PlayDesk"/> class. 
		/// Lay-out is classic Scrabble desk 15*15 + classic bonuses.
		/// </summary>/
		public PlayDesk (Game.Game g) 
		{
			this.game = g;
			this.desk = 	new char[15,15];
			this.wordBonus = new byte[15,15];
			this.charBonus = new byte[15,15];
			
			for(int j=0; j<desk.GetLength(1); j++)
				for(int i=0; i<desk.GetLength(1); i++) {
					desk[i,j] = '_';
					wordBonus[i,j] = 1;
					charBonus[i,j] = 1;
			}
			
			byte[,] tripleW = {{0,0},{7,0},{14,0},{0,7},{7,14},{0,14},{14,0},{14,7},{14,14}};
			byte[,] doubleW = {{1,1},{2,2},{3,3},{4,4},{7,7},{13,1},{12,2},{11,3},{10,4},{4,10},{3,11},{2,12},{1,13},{10,10},{11,11},{12,12},{13,13} };
			byte[,] tripleCh = {{5,1},{9,1},{5,5},{9,5},{1,5},{13,5},{5,9},{9,9},{5,13},{9,13},{1,9},{13,9} };
			byte[,] doubleCh = {{3,0},{11,0},{6,6},{8,6},{6,8},{8,8},{3,14},{11,14},{2,6},{3,7},{2,8},{6,2},{7,3},{8,2},{11,7},{12,6},{12,8},{6,12},{7,11},{8,12} };
			
			LoadFromMemory(wordBonus,tripleW,3);
			LoadFromMemory(wordBonus,doubleW,2);
			LoadFromMemory(charBonus,tripleCh,3);
			LoadFromMemory(charBonus,doubleCh,2);
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="trie.PlayDesk"/> class.
		/// Lay-out is from stream Sr.
		/// </summary>
		/// <param name='sr'>
		/// Sr. Stream with desk
		/// </param>
		public PlayDesk (StreamReader sr)
		{
			//TODO: Loading dictionary via stream reader

		}		

	
	
		
		/// <summary>
		/// Analyzes the move: which letter are need to put, calcul score
		/// </summary>
		/// <returns>
		/// Is move correct?.
		/// </returns>
		/// <param name='M'>
		/// If set to <c>true</c> m.
		/// </param>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		public bool AnalyzeMove(Move M) {
			int i = M.Start.X ;
			int j = M.Start.Y ;
			int n = 0;
			int score = 0;
			int ActualWordBonus = 1;
			
			// 1. Is this real start of word?
			try {
			if( M.Down ) {
				if( desk[i,j-1] != '_' ) return false;
			} else {
				if( desk[i-1,j] != '_' ) return false;
			} }
			catch( System.IndexOutOfRangeException ) { /* ok */ }
				
			// 2. Go through word, now I am at first letter of puted word	
			if( j < 0 || j >= desk.GetLength(1) || i < 0 || i >= desk.GetLength(0)) return false;
			
			bool fail = false;
			while( true ) {
				
				// 2.a) Determinate which letter is need to put
				if( desk[i,j] == '_' ) {
					M.AddLetterToPut( new MovedStone( M.Word[n], i, j ) );
					desk[i,j] = M.Word[n];
					
					int k = Cross(i,j,M.Down);
					if( k < 0 ) {				// Crossword is wrong
						fail = true;
						break; } 	
					if( k == 0) {} 				// No crossword (only this stone)
					score += k;					// K > 0 : K is score for crossword
					
					ActualWordBonus *= wordBonus[i,j];	
					
					score += charBonus[i,j] * desk[i,j].ToRank();
				} else {
					score += desk[i,j].ToRank();
				}
				
				// Prepare to new iteration or break
				n++;
				if( M.Down ) j++; else i++;
				if( j >= desk.GetLength(1) || i >= desk.GetLength(0) ) break;
				if( n == M.Word.Length ) {
					if( desk[i,j] == '_' ) break;
					else {		// Some unknow suffix
						string s = M.Word;
						while( i < desk.GetLength(0) && j < desk.GetLength(1) && desk[i,j] != '_') {
							s += desk[i,j].ToString();
							M.Score += desk[i,j].ToRank();
							if( M.Down ) j++; else i++;
						}
						if( game.dictionary.Content( s ) ) {
							M.Word = s;
							break;
						} else {
							fail = true;
							break;
						}	
					}
				}
			} // end of while cyklus 

			// 3. Delete puted stone (turn is not confirmed
			foreach(MovedStone ms in M.PutedStones ) {
				desk[ms.i, ms.j] = '_';
			}
			
			if( fail ) return false;
			
			score *= ActualWordBonus;
			M.Score = score;
			
		
			
			if( game.dictionary.Content( M.Word ) )	return true;
			else return false;
		}
		
		
		
		/// <summary>
		/// Analyze crossword from position [i, j]. Return value: <0 error;; =0 no word;; <0 score
		/// </summary>
		/// <param name='i'>
		/// 
		/// </param>
		/// <param name='j'>
		/// 
		/// </param>
		/// <param name='down'>
		/// Down (true) or Across (false)
		/// </param>
		private int Cross(int i, int j, bool down) {
			int score = 0;	
			int n = 0;
			Scrabble.Lexicon.Node tmp = game.dictionary.Root;
				
			if( ! down ) { 	
				// 1. find start of crossing word
				while( j > 0 && desk[i,j] != '_' ) {
					j--;	
				}
				if( desk[i,j] == '_' ) j++;
				
				// 2. go throught
				while( j < desk.GetLength(1) && desk[i,j] != '_') {
					if( tmp.isSon( desk[i,j] ) ) {
						tmp = tmp.getSon( desk[i,j] );
					} else tmp = game.dictionary.End;
					score += desk[i,j].ToRank();
					j++;
					n++;
				}
			} else { 
				// 1. find start of crossing word
				while( i > 0 && desk[i,j] != '_' ) {
					i--;	
				}
				if( desk[i,j] == '_' ) i++;
				
				// 2. go throught
				while( i < desk.GetLength(0) && desk[i,j] != '_') {
					if( tmp.isSon( desk[i,j] ) ) {
						tmp = tmp.getSon( desk[i,j] );
					} else tmp = game.dictionary.End;
					score += desk[i,j].ToRank();
					i++;
					n++;
				}
			}
			
			// Result
			if( n == 1 ) return 0;							// only one letter
			if( tmp == game.dictionary.End ) return -1;
			if( tmp.Finite ) return score;
			else return -1;
		}
		
		/// <summary>
		/// Check if new move is connected to other words at play desk.
		/// </summary>
		public bool Connect( Move m ) {
			// Simplest heuristik
			if( m.Word.Length > m.PutedStones.Count ) return true;
			else { 
				
				foreach( MovedStone a in m.PutedStones ) {
					// first move
					if( a.i == 7 && a.j == 7 ) return true; 
					// 
					if( m.Down ) {
						if( this.desk[ a.i+1, a.j ] != '_' ) return true;
						if( this.desk[ a.i-1, a.j ] != '_' ) return true;
					} else {
						if( this.desk[ a.i, a.j+1 ] != '_' ) return true;
						if( this.desk[ a.i, a.j-1 ] != '_' ) return true;						
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Adds the word at the game-logic PlayDesk (not at GTK desk - it call this function when put word)
		/// </summary>
		/// <returns>
		/// Correctness of move
		/// </returns>
		/// <param name='m'>
		/// Move
		/// </param>
		public bool Play(Move move) {
			
			var rack = game.GetActualPlayer().Rack;
			
			// All stones are in rack ? 
			try {
				foreach(MovedStone ms in move.PutedStones ) {
					if( ! rack.Contains( ms.c ) ) return false;
				}
			} catch( System.NullReferenceException) { return false; }
			
			// Put!
			foreach(MovedStone ms in move.PutedStones ) {
				this.desk[ms.i, ms.j] = ms.c;
				rack.Remove( ms.c );
			}
			
			this.game.history.Push( move );
			
			// Increase score
			game.IncActualPlayerScore( move.Score );
		
			return true;
		}
		
		
		/// <summary>
		/// Loads bonuses from memory (array in memory)
		/// </summary>
		/// <param name='to'>
		/// To - target array where are bonuses
		/// </param>
		/// <param name='what1'>
		/// What1 - array of coordinates of bonus
		/// </param>
		/// <param name='what2'>
		/// What2 - concrete bonus (double score, triple score..)
		/// </param>
		private void LoadFromMemory(byte[,] to, byte[,] what1, byte what2) {
			for(int n=0; n<what1.GetLength(0); n++) {
				to[ what1[n,0] , what1[n,1] ] = what2;	
			}
		}
		
		public bool CheckAllDesk() {
			string w = "";
			for( int j=0; j < this.desk.GetLength(1); j++) {
				for( int i=0; i < this.desk.GetLength(0); i++) {
					if( desk[i,j] == '_' ) {
						if( w != "" ) {
							if( ! this.game.dictionary.Content( w ) ) return false;
							w = "";
						}
						continue;
					}
					else w += desk[i,j].ToString();
				}
				if( w != "" ) {
					if( ! this.game.dictionary.Content( w ) ) return false;
					w = "";
				}
			}
			w="";
			for( int i=0; i < this.desk.GetLength(0); i++) {
				for( int j=0; j < this.desk.GetLength(1); j++) {
					if( desk[i,j] == '_' ) {
						if( w != "" ) {
							if( ! this.game.dictionary.Content( w ) ) return false;
							w = "";
						}
						continue;
					}
					else w += desk[i,j].ToString();
				}
				if( w != "" ) {
					if( ! this.game.dictionary.Content( w ) ) return false;
					w = "";
				}
			}
			return true;
		}
		
		public void FindWhichLetterIsNeed(Move m) {
			
		}
	}
	
	public enum UI { Terminal, GTK, log }
}
