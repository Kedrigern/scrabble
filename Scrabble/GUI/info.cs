using System;
using Gtk;
using Scrabble.Game;

namespace Scrabble.GUI
{
	public class Info : Gtk.HBox
	{	
		Gtk.Frame pla;
		Gtk.Frame score;
		Gtk.Table scoresTable;
		Gtk.Label[] scoreValues;
		
		Pango.Layout layout;
		Gtk.DrawingArea da;
		
		Scrabble.Game.Game game;
		
		public Info (Game.Game g)
		{
			pla = new Gtk.Frame("Na tahu");
			pla.BorderWidth = 5;
			score = new Gtk.Frame("Score");
			score.BorderWidth = 5;
			game = g;
			
			this.HeightRequest = 99;
			this.PackStart( pla );
			this.PackEnd( score );
			
			scoresTable = new Gtk.Table( 6, 2, true );
			scoreValues = new Gtk.Label[ 6*2 ];
			for( int i=0; i < this.game.players.Length; i++) {
				scoreValues[ i ] = new Label( game.players[i].Name );
				scoreValues[ i+6 ] = new Label( "0" );
				scoresTable.Attach( scoreValues[i], 0 , 1, (uint) i, (uint) i+1);
				scoresTable.Attach( scoreValues[i+6] , 1, 2, (uint) i, (uint) i+1 );
			}
			for( int i=this.game.players.Length; i < 6; i++) {
				scoreValues[ i ] = new Label( );
				scoreValues[ i+6 ] = new Label( );
				scoresTable.Attach( scoreValues[i], 0 , 1, (uint) i, (uint) i+1);
				scoresTable.Attach( scoreValues[i+6] , 1, 2, (uint) i, (uint) i+1 );
			}
			scoresTable.BorderWidth = 3;
			score.Add( scoresTable );
			
			da = new Gtk.DrawingArea();
			da.ExposeEvent += Expose_Event;	
			pla.Add(da);
			
			layout = new Pango.Layout(this.PangoContext);
			layout.Width = Pango.Units.FromPixels( 90 );
			layout.Wrap = Pango.WrapMode.Word;
			layout.Alignment = Pango.Alignment.Center;
			layout.FontDescription = Pango.FontDescription. FromString("Ahafoni CLM Bold 18");
			
			this.ShowAll();
		}
		
		void Expose_Event(object obj, ExposeEventArgs args){
			layout.SetText( game.GetActualPlayer().Name );
			da.GdkWindow.DrawLayout (da.Style.TextGC (StateType.Normal), 5, 5, layout);
		}
		
		public void Change(string name, Scrabble.Player.Player[] players) {		
			scoresTable.HideAll();
			for( int i=0; i < game.players.Length; i++) {
				if( typeof( Player.ComputerPlayer ) == game.players[i].GetType() )
					scoreValues[ i ].Markup = game.players[i].Name + " <span size=\"smaller\">(CPU)</span>";
				else
					scoreValues[ i ].Text = game.players[i].Name + "  ";
				scoreValues[ i+6 ].Text = string.Format( "{0}", game.players[i].Score );
			}

			scoresTable.ShowAll();
		}
	}
}

