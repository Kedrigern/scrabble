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
using System.IO;
using System.Xml.XPath;
using System.Reflection;

namespace Scrabble.Game
{
	public static class InitialConfig
	{
		public static bool client = false;
		public static bool gui = true;
		public static bool network = false;
		public static bool log;
		public static int numberOfStones;
		public static int sizeOfRack;
		public static int port;
		public static Gdk.Pixbuf icon;
		
		/* Usually complete start window */
		public static Lexicon.GADDAG dictionary;	
		public static Player.Player[] players;
		public static Scrabble.Game.Game game;
		public static System.IO.StreamWriter logStream;
#if DEBUG		
		public static System.IO.StreamWriter logStreamAI;
#endif
		
		/// <summary>
		/// Initializes the <see cref="Scrabble.Game.InitialConfig"/> class (static constructor).
		/// </summary>
		static InitialConfig() {
			try {
				Scrabble.Game.InitialConfig.icon = Gdk.Pixbuf.LoadFromResource( "Scrabble.Resources.icon.svg" );
			} catch {	}	
			
			/* Choice of config */
			string usrShrG = Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ) + "/games/";
			
			if( File.Exists("config.xml") ) {
				XPathDocument xDoc = new XPathDocument( new StreamReader("config.xml") );
				getConfig(xDoc);
			} else if ( File.Exists( usrShrG + "scrabble/config.xml" ) ) {
				XPathDocument xDoc = new XPathDocument(
					Assembly.GetExecutingAssembly().GetManifestResourceStream(usrShrG + "scrabble/config.xml")
				);
				getConfig(xDoc);
			} else {
				XPathDocument xDoc = new XPathDocument(
					Assembly.GetExecutingAssembly().GetManifestResourceStream("Scrabble.Resources.defaultConfig.xml")
				);
				getConfig(xDoc);
			}
		}
		
		static private void getConfig(XPathDocument xDoc) {		
			XPathNavigator xNav = xDoc.CreateNavigator();
					
			sizeOfRack = xNav.SelectSingleNode("/scrabble/game/rackSize").ValueAsInt;
			numberOfStones = xNav.SelectSingleNode("/scrabble/game/numberStones").ValueAsInt;
			
			foreach (XPathNavigator stone in xNav.Select("/scrabble/game/stones/*") ) {
				char c = stone.GetAttribute("type", "")[0];
				int val = int.Parse( stone.GetAttribute("value", "") );
				int count = int.Parse( stone.GetAttribute("count", "") );
				Scrabble.Lexicon.PlayStone.Add(c,val);
				do {
					Scrabble.Game.StonesBag.Add( c );
					count--;
				} while( count <= 0);
			}
			
			log = xNav.SelectSingleNode("/scrabble/technical/log").ValueAsBoolean;
			port = xNav.SelectSingleNode("/scrabble/technical/port").ValueAsInt;			
		}
	}
}

