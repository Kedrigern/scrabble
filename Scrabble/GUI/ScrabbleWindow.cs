using System;
using System.Collections.Generic;
using Gtk;
			
namespace 	Scrabble.GUI {
	
	public partial class 	ScrabbleWindow: Gtk.Window
	{	
		private VBox mainVbox;
		private VBox bottomVbox;
		private HPaned vertical;

		private Statusbar statusbar;
		private Label OnTurnLabel;
		private Label StatusLabelBest;
		private Label StatusLabelLast;
		
		private Scrabble.GUI.Desk desk;
		private Scrabble.GUI.Rack rack;
		private Scrabble.GUI.Info info;	
		private Scrabble.GUI.Control control;
		private Scrabble.GUI.MenuHover menu;
		
		public Scrabble.Game.Game game;
		
		public ScrabbleWindow ( ): base (Gtk.WindowType.Toplevel)
		{		
			this.Title = "Scrabble - Hrací deska";
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.SetPosition( WindowPosition.Center );
			this.DefaultWidth = 550;
			this.DefaultHeight = 650;
			
			if( Scrabble.Game.InitialConfig.game == null ) 
				throw new System.NullReferenceException("During Scrabble main widow initialization is Scrabble.Game.InitialConfig.game == null");
			this.game = Scrabble.Game.InitialConfig.game;	

			menu = new MenuHover( this );
			desk = new Desk( this.game );
			rack = new Rack( this.game );
			control = new Control( this.game );
			info = new Info( this.game );
			
			bottomVbox = new VBox(true, 4 );
			bottomVbox.PackStart( rack );
			bottomVbox.PackEnd( control );
			bottomVbox.ShowAll();
			
			vertical = new HPaned();		
			vertical.HeightRequest = 100;

			statusbar = new Statusbar();
			statusbar.HeightRequest = 20;
			statusbar.Homogeneous = true;
			StatusLabelLast = new Label("Poslední tah:");
			StatusLabelBest = new Label("Nejlepší tah:");
			OnTurnLabel = new Label("Na tahu:");					
			statusbar.PackStart( OnTurnLabel , false, false, 0 );
			statusbar.Add( StatusLabelLast);
			statusbar.PackEnd( StatusLabelBest, false, false, 0 );
			
			vertical = new HPaned();
			vertical.HeightRequest = 100;
			vertical.Add1( bottomVbox );
			vertical.Add2( info );
							
				
			mainVbox = new VBox(false, 5);
			mainVbox.PackStart( menu.menuBar , false, false, 0  );
			mainVbox.Add( desk );
			mainVbox.Add( vertical );
			mainVbox.PackEnd( statusbar, false, false, 0 );
			
			this.ExposeEvent += OnExposeEvent;
			
			this.Add( mainVbox );
			this.changePlayer( game.GetActualPlayer() );			
		}
		
		public void RackChange( List<char> l ) {
			rack.Change( l );
		}
		
		public void changePlayer( Player.Player p ) {
			desk.UpdateDesk( game.desk.Desk );
			rack.Change( ((Player.Player) p).Rack );
			info.Change( p.Name , game.players);
			
			OnTurnLabel.Text = "Na tahu: " + game.GetActualPlayer().Name;
			StatusLabelBest.Text = "Nejlepší tah: " + game.bestMove.Word;
			try {
				StatusLabelLast.Text = "Poslední tah: " + game.historyM.Peek().Word;
			} catch (InvalidOperationException) {
				StatusLabelLast.Text = "Poslední tah: ";
			}
		}
	
		public void DisableButtons() {
			control.DisableButtons();
			desk.DisableButtons();
		}
		
		public void ActiveButtons() {
			control.ActiveButtons();
			desk.ActiveButtons();
		}
		
		public void Restart() {
			this.desk.Restart();
			this.changePlayer( this.game.GetActualPlayer() );
		}
		
		protected void OnExposeEvent ( object sender, ExposeEventArgs a ) {
			this.ErrorBell();
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Gtk.Application.Quit();
			a.RetVal = true;
		}
	}
}
