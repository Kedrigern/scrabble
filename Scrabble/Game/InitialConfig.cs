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

//TODO: Compatibility with last type of config

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
		public static string dicPath;
		public static Gdk.Pixbuf icon;
		
		/* Usually complete start window */
		public static Lexicon.GADDAG dictionary;	
		public static Player.Player[] players;
		public static Scrabble.Game.Game game;
#if DEBUG		
		public static System.IO.StreamWriter logStreamAI;
#endif
		
		/// <summary>
		/// Initializes the <see cref="Scrabble.Game.InitialConfig"/> class (static constructor).
		/// Use config.xml , ~/.scrabble/config.xml, share/scrabble/config.xml or inbuild (defaultConfig.xml from resources)
		/// </summary>
		static InitialConfig() {
			try {
				Scrabble.Game.InitialConfig.icon = Gdk.Pixbuf.LoadFromResource( "Scrabble.Resources.icon.svg" );
			} catch {	}	
			
			#region Choice of config file 
			string usrShrG = Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ) + "/games/";
			string home = Environment.GetFolderPath( Environment.SpecialFolder.Personal ) + "/.scrabble/";
			string config = string.Empty;	
			
			if( File.Exists("config.xml") ) {
				config = "config.xml";
			} else if( File.Exists( home + "config.xml" ) ) {
				config = home + "config.xml";
			} else if ( File.Exists( usrShrG + "scrabble/config.xml" ) ) {
				config = usrShrG + "scrabble/config.xml";
			} 
			
			if ( config == string.Empty ) {
				XPathDocument xDoc = new XPathDocument(
					Assembly.GetExecutingAssembly().GetManifestResourceStream("Scrabble.Resources.defaultConfig.xml")
				);
				getConfig(xDoc);
				if ( log ) {
					Console.Out.WriteLine("[INFO]\tPoužívám: defaultConfig z Resources");
				}
			} else {
				XPathDocument xDoc = new XPathDocument( new FileStream( config, FileMode.Open) );
				getConfig(xDoc);
				if ( log ) {
					Console.Out.WriteLine("[INFO]\tPoužívám: " + config);
				}
			}
			#endregion
			
		}
		
		static private void getConfig(XPathDocument xDoc) {		
			XPathNavigator xNav = xDoc.CreateNavigator();
					
			sizeOfRack = xNav.SelectSingleNode("/scrabble/game/rackSize").ValueAsInt;
			numberOfStones = xNav.SelectSingleNode("/scrabble/game/numberStones").ValueAsInt;
			dicPath = xNav.SelectSingleNode("/scrabble/game/dictionary").Value;
			
			foreach (XPathNavigator stone in xNav.Select("/scrabble/game/stones/*") ) {
				char c = stone.GetAttribute("type", "")[0];
				int val = int.Parse( stone.GetAttribute("value", "") );
				int count = int.Parse( stone.GetAttribute("count", "") );
				Scrabble.Lexicon.PlayStone.Add(c,val);
				do {
					Scrabble.Game.StonesBag.Add( c );
					count--;
				} while( count > 0);
			}
			
			log = xNav.SelectSingleNode("/scrabble/technical/log").ValueAsBoolean;
			port = xNav.SelectSingleNode("/scrabble/technical/port").ValueAsInt;		
						
			if( xNav.SelectSingleNode("/scrabble/technical/debug").ValueAsBoolean ) {
				string path = xNav.SelectSingleNode("/scrabble/technical/debug").GetAttribute("path", "") ;
				StreamWriter sw = new StreamWriter( path );
				sw.AutoFlush = true;
				Console.SetOut( sw );
			} 
		}
	}
}

