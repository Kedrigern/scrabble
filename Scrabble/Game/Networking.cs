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
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Scrabble.Game
{
	public static class Networking
	{		
		public static bool sendInfo( IPEndPoint end ) {
			var client = new TcpClient( end );
			var stream = client.GetStream();
			var encoder = new UTF8Encoding();
			var formatter = new BinaryFormatter();
			
			// greeting
			byte[] buffer = encoder.GetBytes("HELLO");
			stream.Write( buffer,0,buffer.Length);
			stream.Flush();
			
			// response
			stream.Read( buffer, 0, buffer.Length );
			if( encoder.GetString(buffer).StartsWith("ACK") ) {}
			else { return false; }
			
			// send PLAYERS
			client = new TcpClient( end );
			stream = client.GetStream();	
			formatter.Serialize( stream, Scrabble.Game.InitialConfig.players  );			
			
//			// response
//			stream.Read( buffer, 0, buffer.Length );
//			if( encoder.GetString(buffer).StartsWith("ACK") ) {}
//			else { return false; }
//			
//			// send DESK
//			client = new TcpClient( end );
//			stream = client.GetStream();
//			
//			// response	
//			stream.Read( buffer, 0, buffer.Length );
//			if( encoder.GetString(buffer).StartsWith("ACK") ) {}
//			else { return false; }

			return false;
		}
		
		public static  bool sendQuestion( IPAddress endP , out Scrabble.Lexicon.Move m) {
			m = new Scrabble.Lexicon.Move("");
			return false;	
		}
		
		public static bool sendQuit( IPEndPoint endP ) {
			return false;	
		}
		
		public static bool ReciveInfo() {
			UTF8Encoding encoder = new UTF8Encoding();
			BinaryFormatter formatter = new BinaryFormatter();
			bool[] checkpoints = new bool[3]; 						// checkpoint in communication
			TcpListener listener = new TcpListener ( IPAddress.Any, Scrabble.Game.InitialConfig.port );	
			TcpClient client = new TcpClient();
			while( true ) {
				listener.Start();
				client = listener.AcceptTcpClient();				// this function is blocking
				NetworkStream stream = client.GetStream();
				
				// Start of connection
				if( ! checkpoints[0] ) {
					byte[] buffer = new byte[128];
					stream.Read( buffer, 0, buffer.Length );
					string mes = encoder.GetString( buffer );
					if( mes.StartsWith("HELLO") ) {
						//ack
						checkpoints[0] = true;
						continue;
					} else {
						// error		
					}
				}
				
				if( ! checkpoints[1] ) {
					try {
						Scrabble.Player.Player[] pl = (Scrabble.Player.Player[]) formatter.Deserialize( stream );
						Scrabble.Game.InitialConfig.game.players = pl;
						checkpoints[1] = true;
						// ack
					} catch {
						// error						
					}
					
				}
			}
		}
	}
}
