//  
//  searchalgorihm.cs
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
using System.Collections.Generic;
using System.Drawing;

namespace Scrabble.Lexicon
{ 
	/// <summary>
	/// Search algorithm for <see cref="GADDAG"/> and <see cref="PlayDesk"/>.
	/// </summary>
	public static class SearchAlgorithm {
		
		static char[,] desk = new char[0,0];
		static GADDAG gaddag = new GADDAG();
		
		/// <summary>
		/// Init the specified char[,] d and GADDAG g.
		/// </summary>
		/// <param name='d'>
		/// desk to use
		/// </param>
		/// <param name='g'>
		/// GADDAG dictionary to use
		/// </param>
		public static void Init(char[,] d, GADDAG g) {
			SearchAlgorithm.desk =	d;
			SearchAlgorithm.gaddag = g;
		}
		
		/// <summary>
		/// Search at the coordinates x, y. Use letter from rack and result are saved in pool.
		/// </summary>
		/// <param name='x'>
		/// X.
		/// </param>
		/// <param name='y'>
		/// Y.
		/// </param>
		/// <param name='rack'>
		/// Rack.
		/// </param>
		/// <param name='pool'>
		/// Pool.
		/// </param>
		public static void Search( int x, int y, List<char> rack, HashSet<Move> pool)	{
			Node root;
			/* Run only at point:
			 * 		....
			 * 	 	word
			 *		....
			 * (and d)
			 */
			try {
				if( desk[x,y+1] != '_' || 
					desk[x,y-1] != '_' || 
					( desk[x,y] != '_' && desk[x+1,y] == '_' )  ) {
					root = gaddag.ReverseRoot;
					StartOfRecursionLeftRight( pool, root, x, y, rack );
				}
			} catch ( System.IndexOutOfRangeException ) { }
			
			/* Run only at point:
			 * 		.w.
			 * 		.o.
			 * 		.r.
			 * 		.d.
			 * (and d)
			 */
			try {
				if( desk[x+1,y] != '_' ||
					desk[x-1,y] != '_' ||
					( desk[x,y] != '_' && desk[x,y+1] == '_' ) ) {
					root = gaddag.ReverseRoot;
					StartOfRecursionDownUp( pool, root, x, y, rack );
				}
			} catch ( System.IndexOutOfRangeException ) { }
		}
		
		private static void StartOfRecursionLeftRight(HashSet<Move> pool, Node root, int x, int y, List<char> rack ) {	
			string s = "";
			int actual = x;
			// Go over the already puted stone to first '_'
			try {
				while( desk[actual,y] != '_' ) {
					root = root.getSon( desk[actual,y] );
					s = desk[actual,y].ToString() + s;
					actual--;
				}		
				TmpMove tmp = new TmpMove( new Point(x,y), new Point(actual+1,y), 
												Direction.left, s,root, rack );
				SearchRek( pool, tmp );
			} catch (System.IndexOutOfRangeException) {}
		}
		
		private static void StartOfRecursionDownUp(HashSet<Move> pool, Node root, int x, int y, List<char> rack  ) {
			string s = "";
			int actual = y;
			// Go over the already puted stone to first '_'
			try {
				while( desk[x,actual] != '_' ) {
					root = root.getSon( desk[x,actual] );
					s = desk[x,actual].ToString() + s;
					actual--;
				}
			
				TmpMove tmp = new TmpMove( new Point(x,y), new Point(x,actual+1), 
												Direction.up,
												s,root, rack );
				SearchRek( pool, tmp );	
			} catch (System.IndexOutOfRangeException) {}
		}
		
		private static void SearchRek(HashSet<Move> pool, TmpMove m ) {
			// 1. check cross
			if( ! cross( m , m.node.Content ) ) {
				return;
			}
			
			// 2. this is full word - add
			if( m.node.Finite ) {
				pool.Add( m.Convert2Move() ); 
			}
			
			// 3. turn direction of movement
			if( m.node.isSon('>') ) {
				if( m.direct == Direction.left )
					SearchRek( pool, m.TurnRight( ) );
				else 
					SearchRek( pool, m.TurnDown( ) );
			}
			
			// 4. End of desk
			if( EndOfDesk( m ) ) {
				return;		// nothing to do
			}
			
			// 5. Go to next char
			char ch = NextChar(m);
			if( '_' !=  ch ) {	// there are already puted some stone
				if( m.node.isSon( ch ) )	{
					SearchRek( pool, m.Copy( ch ) );	
				}
			} else {			// blank desk (I can put own stone)
				foreach( char c in m.rack ) {
					if( m.node.isSon( c ) ) {
						SearchRek( pool, m.Copy( c ) );	
					}
				}
			}
		}
		
		/// <summary>
		/// Availables the shift.
		/// </summary>
		/// <returns>
		/// Is shift posible?
		/// </returns>
		/// <param name='m'>
		/// Point at desk.
		/// </param>
		private static bool EndOfDesk(TmpMove m) {
			switch( m.direct ) {
			case Direction.left :
				return m.ActualPoint.X -1 < 0 ? true : false;
			case Direction.right :
				return m.ActualPoint.X +1 >= SearchAlgorithm.desk.GetLength(0) ? true : false;
			case Direction.up :
				return m.ActualPoint.Y -1 < 0 ? true : false;
			case Direction.down :
				return m.ActualPoint.Y +1 >= SearchAlgorithm.desk.GetLength(1) ? true : false;
			default :
				return true;
			}
		}
		
		private static char NextChar(TmpMove m) {
			switch( m.direct ) {
			case Direction.left :
				return desk[ m.ActualPoint.X -1, m.ActualPoint.Y];
			case Direction.right :
				return desk[ m.ActualPoint.X +1, m.ActualPoint.Y];
			case Direction.up :
				return desk[ m.ActualPoint.X , m.ActualPoint.Y -1];
			case Direction.down :
				return desk[ m.ActualPoint.X , m.ActualPoint.Y +1];
			default :
				return '_';
			}			
		}
		
		private static bool cross(TmpMove m, char c) {
			int x = m.ActualPoint.X;
			int y = m.ActualPoint.Y;
			switch( m.direct ) {
			case Direction.left :
			case Direction.right :
				// Alone stone
				try {
					if( desk[x,y+1] == 	'_' &&
						desk[x,y-1] == 	'_' ) return true;
				} catch{}
				// Go to begin of word
				while( true ) {
					y--;
					if( y < 0 || desk[x,y] == '_' ) {
						y++;
						break;
					}
				}
				return gaddag.Content(desk, x, y, true, x, y, c);
			case Direction.up :
			case Direction.down :
				// Alone stone
				try {
					if( desk[x+1,y] == 	'_' &&
						desk[x-1,y] == 	'_' ) return true;
				} catch{}
				// Go to begin of word
				while( true ) {
					x--;
					if( x < 0 || desk[x,y] == '_' ) {
						x++;
						break;
					}
				}
				return gaddag.Content(desk, x, y, false, x, y,c);
			default :
				return true;
			}
		}
	}
	
	/// <summary>
	/// Temporaly move - using in search algorithm in GADDAG
	/// </summary>
	class TmpMove {
		public Point LeftStart {get; set; }		// point where start (left start or up start) puted word
		public Point StartPoint {get; set; }	// start point of searching posibilities
		public Point ActualPoint {get; set; }	// actual position in searching
		public Direction direct {get; set; }	// direction of searching (one times change in passing GADDAG)
		public string Word {get; set; }			// created word
		public Node node {get; set; }			// node in lexicon represented by GADDAG
		public List<char> rack;
		
		public TmpMove( TmpMove pm ) {
			LeftStart = pm.LeftStart;
			StartPoint = pm.StartPoint;
			direct = pm.direct;
			rack = pm.rack;	
			Word = pm.Word;
			ActualPoint = pm.ActualPoint;
		}
		public TmpMove(Point start, Point actual, Direction dir2, string w2, Node n2, List<char> r) {
			this.StartPoint = start;
			this.ActualPoint = actual;
			this.LeftStart = Point.Empty;
			this.direct = dir2;
			this.Word = w2;
			this.node = n2;
			this.rack = r;
		}
			
		/// <summary>
		/// Create new instance that is copy of old instance and delete one charakter from rack.
		/// </summary>
		/// <param name='del'>
		/// Char to delete from rack.
		/// </param>
		public TmpMove Copy( char del ) {
			TmpMove pm2 = new TmpMove( this );
			pm2.rack = new List<char>( pm2.rack );
			pm2.rack.Remove( del );
			pm2.node = this.node.getSon( del );
			switch( pm2.direct ) {
			case Direction.left:
				pm2.Word = del + this.Word;
				pm2.ActualPoint = new Point( ActualPoint.X -1, ActualPoint.Y );
				break;
			case Direction.right:
				pm2.Word = this.Word + del;
				pm2.ActualPoint = new Point( ActualPoint.X +1, ActualPoint.Y );
				break;
			case Direction.up:
				pm2.Word = del + this.Word;
				pm2.ActualPoint = new Point( ActualPoint.X, ActualPoint.Y -1 );
				break;
			case Direction.down:
				pm2.Word = this.Word + del;
				pm2.ActualPoint = new Point( ActualPoint.X, ActualPoint.Y +1 );
				break;
			}
			return pm2;
		}

		
		public TmpMove TurnRight() {
			TmpMove pm2 = new TmpMove( this );	
			pm2.direct = Direction.right;
			pm2.LeftStart = new Point( ActualPoint.X, ActualPoint.Y );
			pm2.ActualPoint = new Point( StartPoint.X, StartPoint.Y );
			pm2.node = this.node.getSon('>');
			return pm2;
		}
		
		public TmpMove TurnDown() {
			TmpMove pm2 = new TmpMove( this );	
			pm2.direct = Direction.down;
			pm2.LeftStart = new Point( ActualPoint.X, ActualPoint.Y );
			pm2.ActualPoint = new Point( StartPoint.X, StartPoint.Y );
			pm2.node = this.node.getSon('>');
			return pm2;
		}
		
		public Move Convert2Move() {
			switch ( direct ) {
			case Direction.left:
			case Direction.right:
				return new Move(LeftStart, Word, false );
			case Direction.up:
			case Direction.down:
			default:
				return new Move(LeftStart, Word, true );
			}	
		}
	}
}
