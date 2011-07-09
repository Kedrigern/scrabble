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
		
		public ScrabbleWindow ( ): base (Gtk.WindowType.Toplevel)
		{		
			this.Title = "Scrabble - Hrací deska";
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.SetPosition( WindowPosition.Center );
			this.DefaultWidth = 550;
			this.DefaultHeight = 650;
			
			this.game = Scrabble.Game.InitialConfig.game;	

			menu = new MenuHover( this );
			desk = new Desk( this.game );
			rack = new Rack( this.game );
			info = new Info( this.game );
				
			horizont = new VPaned();
			vertical = new HPaned();
			forMenu = new VBox(false, 5);
			forMenu.PackStart( menu.menuBar , false, false, 0 );
			forMenu.PackEnd( desk );
			this.Add( horizont );
			
			/* Four main GUI components */
			horizont.Add1( forMenu );
			horizont.Add2( vertical );
			vertical.Add1( rack );
			vertical.Add2( info );
				
			this.changePlayer( game.GetActualPlayer() );			
		}
		
		public void RackChange( List<char> l ) {
			rack.Change( l );
		}
		
		public void changePlayer( Player.Player p ) {
			desk.UpdateDesk( game.desk.Desk );
			rack.Change( ((Player.Player) p).Rack );
			info.Change( p.Name , game.players);
		}
	
		public void DisableButtons() {
			rack.DisableButtons();
			desk.DisableButtons();
		}
		
		public void ActiveButtons() {
			rack.ActiveButtons();
			desk.ActiveButtons();
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Gtk.Application.Quit();
			a.RetVal = true;
		}
	}
}