//  
//  InitialConfig.cs
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

namespace Scrabble.Game
{
	public static class InitialConfig
	{
		// TODO : Loading for config file
		public static bool allDone = false;
		public static bool gui = true;
		public static bool network = false;
		public static int numberOfStones = 100;
		public static int sizeOfRack = 7;
		public static int port = 6541;
		public static char[] stones = {
			'A', 'A', 'A', 'A', 'A',
			'Á',
			'B', 'B',
			'C', 'C',
			'Č',
			'D', 'D',
			'E', 'E', 'E', 'E',
			'F', 
			'I', 'I', 'I',
			'Í',
			'J', 'J',
			'K', 'K',
			'L', 'L',
			'M', 'M',
			'N', 'N', 'N',
			'O', 'O', 'O', 'O',
			'P', 'P',
//			'Q',
			'R', 'R',
			'S', 'S', 'S',
			'Š',
			'T', 'T',
			'U', 'U', 'U', 
			'Ú',
			'Ů',
			'V', 'V',
			'X',
			'Y', 'Y',
			'Z', 'Z'
		} ;
		
		/* Usually complete start window */
		public static Lexicon.GADDAG dictionary;	
		public static Player.Player[] players;
		public static Scrabble.Game.Game game;
	}
}

