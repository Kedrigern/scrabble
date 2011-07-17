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
using System.Threading;

namespace Scrabble.Game
{
	/// <summary>
	/// Networking class, do all about communication with rest od player.
	/// It's designed for run in own thread.
	/// </summary>
	public class Networking
	{	
		bool done = false;
		public bool Done { get { return done;} }
		bool client;
		UTF8Encoding encoder = new UTF8Encoding();
		BinaryFormatter formatter = new BinaryFormatter();
		
		public Scrabble.Lexicon.PlayDesk playDesk;
		public Scrabble.Player.Player[] players;
			
		/// <summary>
		/// Initializes a new instance of the <see cref="Scrabble.Game.Networking"/> class.
		/// </summary>
		/// <param name='isClient'>
		/// Is client instance?
		/// </param>
		public Networking( bool isClient ) {
			this.client = isClient;	
		}
		
		/// <summary>
		/// Work this instance (client or server). This is infinite cyklus with Thread.Sleep at the end.
		/// </summary>
		public void work() {
			while ( true ) {	
				#region console1
				Console.WriteLine("Začínám work while");
				#endregion
				done = false;

				if( this.client ) {
					try {
						this.ReciveInfo();	
						this.done = true;
					} catch {}
				} else {
					try {
						foreach ( Player.Player p in this.players )
							if( p.GetType() == typeof( Player.NetworkPlayer ) ) sendInfo( ((Player.NetworkPlayer)p).End );
						this.done = true;
					} catch { }
				}
			
				Thread.Sleep( System.Threading.Timeout.Infinite );
#if DEBUG
				Console.WriteLine( "Probuzen" );
#endif
			}
		}
		
		private bool sendInfo( IPEndPoint end ) {
			var client = new TcpClient( end );
			var stream = client.GetStream();
#if DEBUG
			Console.WriteLine( "Mám stream" );
#endif
			
			// greeting
			byte[] buffer = encoder.GetBytes("HELLO");
			stream.Write( buffer,0,buffer.Length );
			stream.Flush();
			
			// response
			stream.Read( buffer, 0, buffer.Length );
			if( encoder.GetString(buffer).StartsWith("ACK") ) {}
			else { 
				#region console2
				Console.WriteLine( "Nepozdravili jsme se" ); 
				#endregion
				return false; }
			
			// send PLAYERS
			client = new TcpClient( end );
			stream = client.GetStream();	
			formatter.Serialize( stream, Scrabble.Game.InitialConfig.players  );			
			
			// response
			stream.Read( buffer, 0, buffer.Length );
			if( encoder.GetString(buffer).StartsWith("ACK") ) {}
			else { 
				#region console3
				Console.WriteLine( "Nepozdravili jsme se" ); 
				#endregion
				return false; 
			}
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
		
		public  bool sendQuestion( IPAddress endP , out Scrabble.Lexicon.Move m) {
			m = new Scrabble.Lexicon.Move("");
			return false;	
		}
		
		public bool sendQuit( IPEndPoint endP ) {
			return false;	
		}
		
		private bool ReciveInfo() {
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
#if DEBUG
						Console.WriteLine( "HELLO přijate" );
#endif
						ack( stream );
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
						ack( stream );
#if DEBUG
						Console.WriteLine( "Hráči přijati" );
#endif					
					} catch {
						// error						
					}
					
				}
			}			
		}
		
		private void ack( NetworkStream s) {
			byte[] buf = encoder.GetBytes("ACK");
			s.Write( buf, 0, buf.Length);
			s.Flush();
		}
	}
}
