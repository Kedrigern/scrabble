//  
//  PlayStone.cs
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

namespace Scrabble.Lexicon
{
	/// <summary>
	/// Play stones infrastructure
	/// </summary>
	public static class PlayStone
	{
		private static SortedDictionary< char, int > values;
		
		/// <summary>
		/// Static initializes the <see cref="Scrabble.Lexicon.PlayStone"/> class.
		/// </summary>
		static PlayStone() {
			values = new SortedDictionary<char, int>();
		}
		
		/// <summary>
		/// Add the specified stone with letter c and rank (value) i.
		/// </summary>
		/// <param name='c'>
		/// letter at stone
		/// </param>
		/// <param name='i'>
		/// value/rank of stone
		/// </param>
		public static void Add(char c, int i) {
			values.Add(c, i);	
		}
		
		/// <summary>
		/// From char (stone) get yout rank (score)
		/// </summary>
		/// <returns>
		/// The score
		/// </returns>
		/// <param name='c'>
		/// C.
		/// </param>
		public static int ToRank(this char c) {
			try {
				return values[c];
			} catch (KeyNotFoundException) {
				return 1;
			}
		}
	}
}

