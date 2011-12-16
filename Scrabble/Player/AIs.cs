//  
//  AIs.cs
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
using System.Linq;
using System.Collections.Generic;
using Scrabble.Lexicon;

namespace Scrabble.Player {

	public abstract class AI {		
		public abstract decision Decide( HashSet<Move> pool , Move max, out Move dec);	
	}
	
	public class standartAI : AI {
		public override decision Decide (HashSet<Move> pool, Move max, out Move dec)
		{
			dec = new Move("null");
			dec.Score = -1;
			
			if( pool.Count == 0 ) {	// No moves => reloadrack (heuristic)
				return decision.reload;
			}
			
			if( max == null ) {
				//TODO: find max
			} 
			
			dec = max;
			return decision.play;
		}	
	}
	
	public enum decision {
		wait, reload, play	
	}
	
}