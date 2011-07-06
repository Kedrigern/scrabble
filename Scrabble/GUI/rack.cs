using System;
using Gtk;
using System.Collections.Generic;

namespace Scrabble.GUI
{
	public class Rack : Gtk.Frame
	{
		private Gtk.Button[] buttons;
		private Gtk.Button reload;
		private Gtk.Table buttonky;
		private Gtk.VBox vbox;
		private Scrabble.Game.Game game;
		
		public Rack ( Scrabble.Game.Game g ) : base ("Zásobník")
		{
			this.game = g;
			
			vbox = new VBox();
			
			buttons = new Gtk.Button[ Game.InitialConfig.sizeOfRack ];
			buttonky = new Gtk.Table(1, (uint) Game.InitialConfig.sizeOfRack , true);
			for(uint i=0; i< Game.InitialConfig.sizeOfRack; i++) {
				buttons[i] = new Gtk.Button();
				buttonky.Attach( buttons[i] ,  i, i+1, 0, 1 );
			}
			
			buttonky.ColumnSpacing = 6;
			buttonky.BorderWidth = 3;
			this.BorderWidth = 5;
			
			reload = new Gtk.Button("Reload");
			reload.Clicked += ReloadRack;
			
			vbox.PackStart( buttonky );
			vbox.Spacing = 15;
			vbox.PackEnd( reload );
			
			this.Add( vbox );
		}
		
		/// <summary>
		/// Change content of visual rack to l.
		/// </summary>
		/// <param name='l'>
		/// L.
		/// </param>
		public void Change(List<char> l) {
			for(int i=0; i	< l.Count; i++) {
				((Gtk.Button) buttons[i]).Label = l[i].ToString();
				buttons[i].Show();
			}
			// Empty slot of rack
			for(int i=l.Count ; i < Game.InitialConfig.sizeOfRack; i++) {
				buttons[i].Hide();
			}
		}
		
		public void DisableButtons() {
			reload.Clicked -= ReloadRack;	
		}
		
		public void ActiveButtons() {
			reload.Clicked += ReloadRack;
		}
		
		private void ReloadRack(object sender, EventArgs e) {
			this.game.ReloadRackAndChange();
		}
	}
}

