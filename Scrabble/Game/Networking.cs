//  
//  Networking.cs
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
using System.Net;
using System.Net.Sockets;

namespace Scrabble.Game
{
	public enum netOp { info, questionToMove }
	
	public class Networking
	{
		
		
		public Networking ( )
		{
		}
		
		public bool sendInfo( IPEndPoint endP ) {
			return false;
		}
		
		public bool sendQuestion( IPEndPoint endP ) {
			return false;	
		}
		
		public bool sendQuit( IPEndPoint endP ) {
			return false;	
		}
	}
}

