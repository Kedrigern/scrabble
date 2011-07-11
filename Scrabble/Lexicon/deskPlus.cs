//  
//  deskPlus.cs
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

namespace Scrabble.Lexicon
{
	/// <summary>
	/// Desk plus is collection of extendet methods for char[,]
	/// </summary>
	public static class deskPlus
	{
		/// <summary>
		/// Return deep copy of this instance (no reference)
		/// </summary>
		/// <param name='d'>
		/// D.
		/// </param>
		public static char[,] DeepCopy( this char[,] d)
		{
			char[,] d2 = new char[ d.GetLength(0) , d.GetLength(1) ];
			for( int j = 0; j < d.GetLength(1); j++)
				for( int i = 0; i < d.GetLength(0); i++ )
					d2[i,j] = d[i,j];
			return d2;
		}
		
		
		/// <summary>
		/// Check if two arrays has same values.
		/// </summary>
		/// <returns>
		/// The values.
		/// </returns>
		/// <param name='d'>
		/// If set to <c>true</c> d.
		/// </param>
		/// <param name='d2'>
		/// If set to <c>true</c> d2.
		/// </param>
		public static bool SameValues( this char[,] d, char[,] d2 ) {
			if( d.GetLength(0) != d2.GetLength(0) ) return false;
			if( d.GetLength(1) != d2.GetLength(1) ) return false;
			for( int j = 0; j < d.GetLength(1); j++)
				for( int i = 0; i < d.GetLength(0); i++ )
					if( d2[i,j] != d[i,j] ) return false;
			return true;						
		}
	}
}

