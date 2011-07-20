using System;
using Gtk;
using System.Collections.Generic;
using Scrabble.Lexicon;

namespace Scrabble.GUI
{
	/// <summary>
	/// Gui rack, show rack (letters and their values)
	/// </summary>
	public class Rack : Gtk.Frame
	{
		private Gtk.Label[] labels;
		private Gtk.Table labelsHover;
		
		public Rack ( Scrabble.Game.Game g ) : base ("Zásobník")
		{
			this.HeightRequest = 60;
			this.WidthRequest = 350;
			
			this.labels = new Gtk.Label[ Scrabble.Game.InitialConfig.sizeOfRack ];
			this.labelsHover = new Gtk.Table(1, (uint) Game.InitialConfig.sizeOfRack , true);
			for(uint i=0; i< Game.InitialConfig.sizeOfRack; i++) {
				this.labels[i] = new Label();
				this.labelsHover.Attach( labels[i] ,  i, i+1, 0, 1 );
			}
			
			this.labelsHover.ColumnSpacing = 6;
			this.labelsHover.BorderWidth = 3;
			this.BorderWidth = 5;
						
			this.Add( this.labelsHover );
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
			}
			// Empty slot of rack
			for(int i=l.Count ; i < Game.InitialConfig.sizeOfRack; i++) {
				this.labels[i].Hide();
			}
		}
	}
}