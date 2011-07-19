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
	
	public class scrabbleClient {
		public bool newData = false;
		public bool yourTurn = false;
		
		TcpListener listener;
		TcpClient client;
		UTF8Encoding encoder = new UTF8Encoding();
		BinaryFormatter formatter = new BinaryFormatter();
		NetworkStream stream;
		byte[] buffer;
		
		Scrabble.Game.Game game;
		bool end = false;
		
		public scrabbleClient( Scrabble.Game.Game g ) {
			this.game = g;
			this.listener = new TcpListener (IPAddress.Any, Scrabble.Game.InitialConfig.port );
		}
		
		public void mainLoop ( ) {
			this.listener.Start();
			while ( ! end ) {
				Recive();
			}	
		}
		
		private void Recive() {
			
			this.client = listener.AcceptTcpClient();
			this.stream = this.client.GetStream();
			
			this.buffer = new byte[4];
			this.stream.Read( buffer, 0, buffer.Length );
			string mes = encoder.GetString( buffer );
			
			if( mes.StartsWith( "FULL" ) ) {
#if DEBUG
				Console.WriteLine("Přijimam FULL update");
#endif
				NetworkCarrierFull c = ( NetworkCarrierFull ) formatter.Deserialize( this.stream );
				this.game.networkUpdate( c );
			}
			
			if( mes.StartsWith( "MINI" ) ) {
#if DEBUG
				Console.WriteLine("Přijimam MINI update");
#endif
				NetworkCarrierMini c = ( NetworkCarrierMini ) formatter.Deserialize( this.stream );
				this.game.networkUpdate( c );
			}
			
			if( mes.StartsWith( "MOVE" ) ) {
				NetworkCarrierPlayer c = (NetworkCarrierPlayer) formatter.Deserialize( this.stream );
				lock( this.game.gameLock ) {
					this.game.yourTurn = true;
					this.game.ncp = c;
				}
				
				while( true ) {
					try {
						Thread.Sleep( Timeout.Infinite );
					} catch (ThreadInterruptedException) {}
					lock( this.game.gameLock ) {
						if( this.game.turnDone ) {
							this.game.turnDone = false;
							this.game.yourTurn = false;
							formatter.Serialize( this.stream, this.game.move);
							this.stream.Flush();
							this.stream.Close();
							break;
						}
							
					}
				}
			}
			
			if( mes.StartsWith( "EXIT" ) ) {
				this.listener.Stop();
				try { this.stream.Close(); } catch {}
				try { this.client.Close(); } catch {}
				this.end = true;
			}
			
		}
		
		public void setNewData() {
				
		}
	}
	
	public class scrabbleServer {
		TcpClient client;
		UTF8Encoding encoder = new UTF8Encoding();
		BinaryFormatter formatter = new BinaryFormatter();
		NetworkStream stream;
		byte[] buffer;
		
		Scrabble.Game.Game game;
		
		public scrabbleServer( Scrabble.Game.Game g ) {
			this.game = g;
		}
		
		public void sendFullInfo(string ip) {
			NetworkCarrierFull c = new NetworkCarrierFull( this.game.players, this.game.desk );
			newConnection(ip, "FULL");
			try {
				this.formatter.Serialize( this.stream, c );
				return;
			} catch (Exception e) {
				Console.WriteLine( e.Message );
			}	
		}
		
		public void sendMiniInfo(string ip, NetworkCarrierMini cin = null) {
			NetworkCarrierMini c;
			if ( cin == null ) {
				int[] ints = new int[ this.game.players.Length ];
				for(int i=0; i< this.game.players.Length; i++) {
					ints[i] = this.game.players[i].Score;	
				}
				c = new NetworkCarrierMini( ints , this.game.desk.Desk );
			} else {
				c = cin;
			}
			newConnection(ip, "MINI");
			try {
				this.formatter.Serialize( this.stream, c );
				return;
			} catch (Exception e) {
				Console.WriteLine( e.Message );
			}	
		}
		
		public void sendQuestion(string ip) {
			newConnection(ip, "MOVE");
			try {
				this.formatter.Serialize( this.stream, this.game.getNCP() );
				this.stream.Flush();
				Lexicon.Move m = (Lexicon.Move) this.formatter.Deserialize( this.stream );
				this.game.desk.Play( m );
			} catch (Exception e ) {
				Console.WriteLine( e.Message );
			}
		}
		
		public void sendExit(string ip) {
			newConnection(ip, "EXIT");
		}
		
		private void newConnection( string ip, string s ) {
			int n =0;
			while( true ) {
				try {
					this.client = new TcpClient( ip , Scrabble.Game.InitialConfig.port);
					break;
				} catch ( System.Net.Sockets.SocketException ) {
					Console.WriteLine("[info]\tNepodařilo se spojit (zbývá {0} pokusů).", 4-n);
					n++;
					if( n == 5 ) {
						Scrabble.Game.InitialConfig.logStream.WriteLine("Nedaří se spojit s: {0}", ip);
						Scrabble.Game.InitialConfig.logStream.Flush();
						Environment.Exit(0);
					}
					System.Threading.Thread.Sleep( 2000 );
				}
			}
			this.buffer = this.encoder.GetBytes( s );
			this.stream = this.client.GetStream();
			this.stream.Write( this.buffer, 0, this.buffer.Length );
			this.stream.Flush();
		}
	}
	
	#region network infrastructure
	public enum carrierType { mini, full }
	
	[Serializable]
	public abstract class NetworkCarrier {}
	
	[Serializable]
	public class NetworkCarrierFull : NetworkCarrier {
		public Scrabble.Player.Player[] players;	
		public Scrabble.Lexicon.PlayDesk playDesk;
		public NetworkCarrierFull(Scrabble.Player.Player[] p, Scrabble.Lexicon.PlayDesk d) {
			this.players = p;
			this.playDesk = d;
		}
	}
	
	[Serializable]
	public class NetworkCarrierMini : NetworkCarrier {
		public int[] scores;
		public char[,] desk;
		public NetworkCarrierMini(int[] s, char[,] d) {
			this.scores = s;
			this.desk =d;
		}
	}
	
	[Serializable]
	public class NetworkCarrierPlayer : NetworkCarrier {
		public int order;
		public System.Collections.Generic.List<char> rack;
		public NetworkCarrierPlayer( int o, System.Collections.Generic.List<char> l) {
			this.order = o;
			this.rack = l;
		}
	}
	#endregion
}
