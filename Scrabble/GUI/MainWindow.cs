using System;
using System.Collections.Generic;
using Gtk;
			
namespace 	Scrabble.GUI {
	
	public partial class 	ScrabbleWindow: Gtk.Window
	{						
		private VPaned horizont;	// first vertical line 
		private HPaned vertical;
		private VBox forMenu;

		private Scrabble.GUI.Desk desk;
		private Scrabble.GUI.Rack rack;
		private Scrabble.GUI.Info info;	
		private Scrabble.GUI.MenuHover menu;
		
		public Scrabble.Game.Game game;
		
		public ScrabbleWindow ( Game.Game g  ): base (Gtk.WindowType.Toplevel)
		{
			this.game = g;
			this.BorderWidth = 0;
			this.Name = "Scrabble";
			
			menu = new MenuHover( this );
			desk = new Desk( g.desk );
			rack = new Rack( g );
			info = new Info( g );
				
			horizont = new VPaned();
			vertical = new HPaned();
			forMenu = new VBox(false,5);
			forMenu.PackStart( menu.menuBar , false, false, 0 );
			forMenu.PackEnd( desk );
			this.Add( horizont );
			
			/* Four main GUI components */
			horizont.Add1( forMenu );
			horizont.Add2( vertical );
			vertical.Add1( rack );
			vertical.Add2( info );
			this.ShowAll();
		}
		
		public void RackChange( List<char> l ) {
			rack.Change( l );
		}
		
		public void changePlayer( Player.Player p ) {
			rack.Change( ((Player.Player) p).Rack );
			info.Change( p.Name , game.players);
		}
	
		
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.Dispose();
			Application.Quit ();
			a.RetVal = true;
		}
	}
}