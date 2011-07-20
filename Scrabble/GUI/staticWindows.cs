//  
//  staticWindows.cs
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
using Gtk;
using Gdk;

namespace Scrabble.GUI
{
	/// <summary>
	/// Small static windows. They are using from diffent part of code, general from menu
	/// </summary>
	class StaticWindows {
		private static Scrabble.Game.Game game;
	
		public static void Init( Scrabble.Game.Game g ) {
			game = g;
		}
		
		public static void DictionaryInfoDialog(object sender, EventArgs e) {
			Gtk.MessageDialog md = new Gtk.MessageDialog( 
								game.Window, 
								DialogFlags.DestroyWithParent,
								MessageType.Info,
								ButtonsType.Ok, 
								"Slovník obsahuje:\n"+ game.dictionary.WordCount+
								"\t\tslov\n"+game.dictionary.NodeCount+
								"\t\tvrcholů\nNejdelší slovo má delku "+ game.dictionary.MaxDepth+".");
			md.Run();
			md.Hide();
			md.Destroy();	
		}
		
		public static void CheckWordDialog(object sender, EventArgs e) {
			var lab = new Gtk.Label("Zadejte slovo: ");
			var ent = new Gtk.Entry();
			var but = new Gtk.Button("OK");
			var div = new Gtk.HBox(false, 1 );
			div.PackStart( lab );
			div.Add( ent );
			div.PackEnd( but );
			var checkWin = new Gtk.Window( Gtk.WindowType.Popup );
			checkWin.Add ( div );
			checkWin.BorderWidth = 0;
			checkWin.Modal = true;
			checkWin.CanFocus = true;
			checkWin.SetPosition( WindowPosition.Mouse );
			checkWin.ShowAll();	
			ent.Activated += delegate {
				but.Click();
			}; 
			but.Clicked += delegate {	
				checkWin.HideAll();

				if( game.dictionary.Content( ent.Text ) ) {	
					Gtk.MessageDialog ans = new Gtk.MessageDialog(
							game.Window,
							DialogFlags.DestroyWithParent,
							MessageType.Info,
							ButtonsType.Close,
							"Slovo \""+ent.Text+"\" <b>je</b> ve slovníku"
						);
					ans.Run();
					ans.Destroy();
				}
				else {
					Gtk.MessageDialog ans = new Gtk.MessageDialog(
							game.Window,
							DialogFlags.DestroyWithParent,
							MessageType.Info,
							ButtonsType.Close,
							"Slovo \""+ent.Text+"\" <b>není</b> ve slovníku"
						);					
					ans.Run();
					ans.Destroy();
				}
				checkWin.Dispose();
				checkWin.Destroy();				
			};
			
			checkWin.KeyPressEvent += delegate(object o, KeyPressEventArgs args) {
				switch( args.Event.Key ) {
				case Gdk.Key.Escape:
					checkWin.HideAll();
					checkWin.Dispose();
					checkWin.Destroy();
					break;
				case Gdk.Key.ISO_Enter:
					but.Click();
					break;
				}
			};	
		}
		
		public static void AddNewWordToDic(object sender, EventArgs e) {
			var win = new Gtk.Window("Přidej slovo");
			win.SetPosition( WindowPosition.Mouse );
			Label l = new Label();
			l.Text = "Vloží slovo do aktuálně načteného slovníku, avšak nezmění zdroj (např. soubor dic.txt )";

			Entry entry = new Entry();
			Button b = new Button("Přidej");
			VBox vbox = new VBox();
			HBox hbox = new HBox();
			vbox.BorderWidth = 10;
			
			vbox.PackStart( l );
			vbox.PackEnd( hbox );
			
			hbox.PackStart( entry );
			hbox.PackEnd( b );
			
			b.Clicked += delegate {
				game.dictionary.Add( entry.Text );
				win.HideAll();
				win.Destroy();
				win.Dispose();
			};
			
			win.Add(vbox);
			win.ShowAll();
			
		}
	
		public static void LoadNewDictionaryDialog(object sender, EventArgs e ) {
			var fch = new Gtk.FileChooserDialog( "Vyberte slovník", null, FileChooserAction.Open, 
												Stock.Open, ResponseType.Ok,
												Stock.Cancel, ResponseType.Cancel);
			FileFilter ff = new FileFilter();
			ff.AddPattern("*.txt");
			ff.Name = "Slovník";
			fch.AddFilter(ff);
			
			try { 
				ResponseType choice = (ResponseType) fch.Run(); 
				if( choice == ResponseType.Ok ) {
					System.IO.StreamReader sr = new System.IO.StreamReader( fch.Filename );
					game.dictionary = new Scrabble.Lexicon.GADDAG( sr );
					MessageDialog info = new MessageDialog( null, DialogFlags.Modal, MessageType.Info,
														ButtonsType.Close, false, null );
					info.Text = "Slovník úspěšně načten. Obsahuje:\n"+ 
								game.dictionary.WordCount +"\t\t slov\n"+ 
								game.dictionary.NodeCount +"\t\t vrcholů";
					info.Run();
					info.Hide();
					info.Destroy();
				}
			}
			catch {  }
			finally { fch.Destroy(); }
		}
		
		public static void NextPlayer(string name) {
			var but = new Gtk.Button( );
			but.TooltipMarkup = "Po kliknutí bude hrát další hráč";
			HBox hbox = new HBox();
			global::Gtk.Image im = new global::Gtk.Image ();
			Label l = new Label();
			l.Markup = "Na tahu je hráč:\n <b>" + name + "</b>\n\nOK";
			hbox.PackStart( im );
			hbox.PackEnd( l );
			but.Add( hbox );
			var win = new Gtk.Window( Gtk.WindowType.Toplevel );	
			
			but.Clicked += delegate {
				win.HideAll();
				win.Dispose();
				win.Destroy();
			};
			win.Add( but );
			win.Fullscreen();
			win.ShowAll();			
		}
		
		
		public static void AboutProgramDialog(object sender, EventArgs e) {
			string text = "Vše vzniklo jako ročníkový projekt na MFF UK v roce 2011.\n"+
					"Implementačním jazykem je C#, grafickou knihovnou GTK#";
			Gtk.AboutDialog aboutWin = new Gtk.AboutDialog();
			aboutWin.Copyright = "GPL";
			aboutWin.Documenters = new string[] {"Ondřej Profant"};
			aboutWin.ProgramName = "gScrabble";
			aboutWin.Authors = new string[] {"Ondřej Profant"};
			aboutWin.Artists = new string[] {"Ondřej Profant", "Lada Švadlenková"};
			aboutWin.Website = "https://github.com/Kedrigern/scrabble";
			aboutWin.Title = "O programu gScrabble";
			aboutWin.WebsiteLabel = "Projekt na GitHubu";
			aboutWin.WrapLicense = true;
			aboutWin.Logo = Scrabble.Game.InitialConfig.icon;
			aboutWin.Comments = text;
			aboutWin.Run();
			aboutWin.Hide();
			aboutWin.Destroy();
		}
	}
}

