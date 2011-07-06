//  
//  StartGameWindow.cs
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
using Scrabble.Game;

namespace Scrabble.GUI
{	
	public class StartGameWindow : Gtk.Window
	{
		int numberOfPlayer = 2;
		Game.Game game;

		/* first variant */
		Gtk.HBox mainHbox;
		Gtk.Button ok;
		Gtk.SpinButton spinBut;
		
		/* second variant */
		Gtk.VBox main2Vbox;	
		Gtk.FileChooserButton dic;
		Scrabble.GUI.PlayerInit[] players;
		
		
		public StartGameWindow (Game.Game g) : base ( Gtk.WindowType.Toplevel )
		{
			game = g;
			
			mainHbox = new Gtk.HBox();		
			Gtk.Label l = new Gtk.Label("Zadejte počet hráčů: ");
			spinBut = new Gtk.SpinButton( numberOfPlayer, Game.Game.MaxNumberOfPlayers , 1 );
			ok = new Gtk.Button("ok");
			ok.Clicked += OkClicked;
			
			mainHbox.PackStart( l );
			mainHbox.Add( spinBut );
			mainHbox.PackEnd( ok );
			mainHbox.BorderWidth = 10;
			mainHbox.Spacing = 5;
			
			this.Add( mainHbox );
			this.DefaultHeight = 50;
			this.SetPosition( Gtk.WindowPosition.Center );
			this.Visible = true;	
			this.Modal = true;
			this.ActivateFocus();
			
			this.ShowAll();			
		}
		
		protected void OkClicked(object sender, EventArgs e) {
			numberOfPlayer = spinBut.ValueAsInt;
			change();
		}
		
		private void change() {
			mainHbox.HideAll();
			this.Remove(mainHbox);
			
			main2Vbox = new Gtk.VBox();
			main2Vbox.Spacing = 7;
			main2Vbox.BorderWidth = 12;
			
			Gtk.HBox hbox = new Gtk.HBox();
			dic = new Gtk.FileChooserButton("Vyberte slovník",Gtk.FileChooserAction.SelectFolder,"Vyberte slovník");
			hbox.PackStart( new Gtk.Label("Vyberte slovník: ") );
			hbox.PackEnd( dic );
			main2Vbox.PackStart( hbox );
			
			main2Vbox.Add(new Gtk.Label("(pokud nevyberete použije se předdefinovaný)") );
			
			players = new PlayerInit[numberOfPlayer];
			for( int i=0; i < numberOfPlayer; i++ ) {
				players[i] = new PlayerInit();
				main2Vbox.Add( players[i] );
			}
			ok = new Gtk.Button("OK");
			ok.Clicked += Done;
			main2Vbox.PackEnd(ok);
			this.Add( main2Vbox );
			main2Vbox.ShowAll();
		}
		
		private void Done(object sender, EventArgs e) {
			//TODO: Dictionary...
			this.HideAll();
			((Scrabble.Game.Game) game).SetPlayers( players );
			((Scrabble.Game.Game) game).PrepareDictionary();
			((Scrabble.Game.Game) game).CreateMainWindowLoop();	
			this.Destroy();
			this.Dispose();			
		}
	}
	
	public class PlayerInit : Gtk.HBox {		
		public Gtk.CheckButton checkCpu;
		Gtk.Label label;
		public Gtk.Entry entry;
		
		public PlayerInit() {
			checkCpu = new Gtk.CheckButton("CPU");
			this.PackEnd( checkCpu );
			label = new Gtk.Label("Jméno:");
			this.PackStart( label );
			entry = new Gtk.Entry();
			this.Add( entry);
		}
		
	}
}

