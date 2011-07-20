//  
//  Control.cs
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

namespace Scrabble.GUI
{
	public class Control : Gtk.Frame
	{
		private Gtk.VBox mainVbox;
		private Gtk.Button reload;
		private Gtk.Button pass;
		
		private Scrabble.Game.Game game;
		
		public Control ( Scrabble.Game.Game g) : base ("Tah")
		{
			this.HeightRequest = 60;
			this.WidthRequest = 350;
			
			
			this.game = g;
			this.BorderWidth = 5;
			
			reload = new Gtk.Button("Reload");
			reload.Clicked += ReloadRack;
			pass = new Gtk.Button("Vzdát se tahu");
			pass.Clicked += delegate {
				this.game.changePlayer();
			};
			
			mainVbox = new Gtk.VBox();
			mainVbox.PackStart( reload );
			mainVbox.PackEnd( pass );
			
			this.Add( mainVbox );
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

