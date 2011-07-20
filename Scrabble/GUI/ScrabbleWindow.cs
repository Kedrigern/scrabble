using System;
using System.Collections.Generic;
using Gtk;
			
namespace Scrabble.GUI {
	
	/// <summary>
	/// Main Scrabble window, show desk, game controls, menu and allows user interaction
	/// </summary>
	/// <exception cref='System.NullReferenceException'>
	/// Is thrown when there is an attempt to dereference a null object reference.
	/// </exception>
	public partial class 	ScrabbleWindow: Gtk.Window
	{	
		private VBox mainVbox;
		private VBox bottomVbox;
		private HPaned vertical;

		private Statusbar statusbar;
		private Label OnTurnLabel;
		private Label StatusLabelBest;
		private Label StatusLabelLast;
		private Label clientNotice;

		private Scrabble.GUI.Desk desk;
		private Scrabble.GUI.Rack rack;
		private Scrabble.GUI.Info info;	
		private Scrabble.GUI.Control control;
		private Scrabble.GUI.MenuHover menu;
		
		private bool client;
		
		public Scrabble.Game.Game game;
		public bool end = false;
		
		public ScrabbleWindow ( bool isClient = false ): base (Gtk.WindowType.Toplevel)
		{		
			#region Basic window properties
			// Basic window properties
			this.Title = "Scrabble - Hrací deska" + (isClient ? " (klient)" : "");
			this.Name = "MainWindow";
			this.DoubleBuffered = true;
			this.SetPosition( WindowPosition.Center );
			try {
				this.Icon = new Gdk.Pixbuf("gscrabble.svg");
			} catch {}
			
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);

			this.DefaultWidth = 550;
			this.DefaultHeight = 700;
			#endregion
			
			this.client = isClient;
			if( Scrabble.Game.InitialConfig.game == null ) 
				throw new System.NullReferenceException("During Scrabble main widow initialization is Scrabble.Game.InitialConfig.game == null");
			this.game = Scrabble.Game.InitialConfig.game;	
			
			// General GUI classes
			this.menu = new MenuHover( this );
			this.desk = new Desk( this.game );
			this.rack = new Rack( this.game );
			this.control = new Control( this.game );
			this.info = new Info( this.game );
			
			this.bottomVbox = new VBox(true, 4 );
			this.bottomVbox.PackStart( rack );
			this.bottomVbox.PackEnd( control );
			this.bottomVbox.ShowAll();
			
			this.vertical = new HPaned();		
			this.vertical.HeightRequest = 100;
			
			// Use only CLIENT
			clientNotice = new Label();
			clientNotice.Markup = "<b>Čekám</b> na aktualizaci dat o hře.";
			clientNotice.TooltipText = "Na portu " + Scrabble.Game.InitialConfig.port.ToString();
			
			// STATUSBAR
			statusbar = new Statusbar();
			statusbar.HeightRequest = 20;
			statusbar.Homogeneous = true;
			StatusLabelLast = new Label("Poslední tah:");
			StatusLabelBest = new Label("Nejlepší tah:");
			OnTurnLabel = new Label("Na tahu:");					
			statusbar.PackStart( OnTurnLabel , false, false, 0 );
			statusbar.Add( StatusLabelLast);
			statusbar.PackEnd( StatusLabelBest, false, false, 0 );
			
			// Vertical divide at bottom
			this.vertical = new HPaned();
			this.vertical.HeightRequest = 100;
			this.vertical.Add1( bottomVbox );
			this.vertical.Add2( info );
							
				
			this.mainVbox = new VBox(false, 5);
			if( isClient )
				this.mainVbox.PackStart( clientNotice, false, false , 5);
			else
				this.mainVbox.PackStart( menu.menuBar , false, false, 0  );
			this.mainVbox.Add( desk );
			this.mainVbox.Add( vertical );
			this.mainVbox.PackEnd( statusbar, false, false, 0 );
				
			this.Add( mainVbox );
			this.changePlayer( game.GetActualPlayer() );			
			
			if( isClient ) DisableButtons();
			
		}
		
		/// <summary>
		/// Change GUI rack (class which show rack to user)
		/// </summary>
		public void RackChange( List<char> l ) {
			this.rack.Change( l );
		}
		
		/// <summary>
		/// Update statusbar ( best move, last move, actual player)
		/// </summary>
		public void changePlayer( Player.Player p ) {
			this.desk.UpdateDesk( game.desk.Desk );
			this.rack.Change( ((Player.Player) p).Rack );
			this.info.Change( p.Name , game.players);
			
			this.OnTurnLabel.Text = "Na tahu: " + game.GetActualPlayer().Name;
			this.StatusLabelBest.Text = "Nejlepší tah: " + game.bestMove.Word;
			try {
				this.StatusLabelLast.Text = "Poslední tah: " + game.historyM.Peek().Word;
			} catch (InvalidOperationException) {
				this.StatusLabelLast.Text = "Poslední tah: ";
			}
		}
	
		/// <summary>
		/// Disables the buttons (delete events) and hide rack and user control
		/// </summary>
		public void DisableButtons() {
			this.control.DisableButtons();
			this.desk.DisableButtons();		
			this.rack.Hide();
			this.control.Hide();
			if( this.client ) {
				this.clientNotice.Markup = "<b>Čekám</b> na aktualizaci dat o hře."	;
			}
		}
		
		/// <summary>
		/// Actives the buttons (add events) and show rack and user control
		/// </summary>
		public void ActiveButtons() {
			this.control.ActiveButtons();
			this.desk.ActiveButtons();
			if( this.client ) {
				this.clientNotice.Markup = "<b>Jste na tahu !</b>"	;
			}
			this.rack.Show();
			this.control.Show();
		}
		
		
		/// <summary>
		/// Update scrabble window (clear desk and put new data from logic desk). 
		/// </summary>
		public void Update() {
			this.desk.Restart();
			this.changePlayer( this.game.GetActualPlayer() );
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.end = true;
		}
	}
}
