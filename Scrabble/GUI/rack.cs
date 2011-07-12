using System;
using Gtk;
using System.Collections.Generic;
using Scrabble.Lexicon;

namespace Scrabble.GUI
{
	public class Rack : Gtk.Frame
	{
		private Gtk.Button[] buttons;
		private Gtk.Label[] labels;
		private Gtk.Table buttonky;
		private Gtk.VBox vbox;
		private Scrabble.Game.Game game;
		
		public Rack ( Scrabble.Game.Game g ) : base ("Zásobník")
		{
			this.game = g;
			this.HeightRequest = 99;
			
			this.vbox = new VBox();
			
			this.buttons = new Gtk.Button[ Game.InitialConfig.sizeOfRack ];
			this.labels = new Gtk.Label[ Scrabble.Game.InitialConfig.sizeOfRack ];
			buttonky = new Gtk.Table(1, (uint) Game.InitialConfig.sizeOfRack , true);
			for(uint i=0; i< Game.InitialConfig.sizeOfRack; i++) {
				labels[i] = new Label();
				buttons[i] = new Gtk.Button();
				buttonky.Attach( labels[i] ,  i, i+1, 0, 1 );
			}
			
			buttonky.ColumnSpacing = 6;
			buttonky.BorderWidth = 3;
			this.BorderWidth = 5;

			
			vbox.PackStart( buttonky );
			vbox.Spacing = 15;
			
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
				this.labels[i].Markup = "<b>" + l[i].ToString() +"</b><sub>("+ l[i].ToRank() + ")</sub>";
				this.labels[i].Show();
				this.labels[i].TooltipText = "Písmeno " + l[i].ToString() + " je ve vašem zásobníku a má hodnotu: " + l[i].ToRank();
				//((Gtk.Button) buttons[i]).Label = l[i].ToString();
				//buttons[i].Show();
			}
			// Empty slot of rack
			for(int i=l.Count ; i < Game.InitialConfig.sizeOfRack; i++) {
				labels[i].Hide();
				//buttons[i].Hide();
			}
		}
	}
}

