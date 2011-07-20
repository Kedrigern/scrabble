using System;
using Gtk;
using Scrabble.Game;

namespace Scrabble.GUI
{
	public class Info : Gtk.Frame
	{	
		Gtk.Table scoresTable;
		Gtk.Label[] scoreValues;
		
		Scrabble.Game.Game game;
		
		public Info (Game.Game g)
		{

			this.Label = "Score";
			this.BorderWidth = 5;
			this.game = g;
			
			this.HeightRequest = 99;
			this.WidthRequest = 120;
			
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
			this.Add( scoresTable );

			this.ShowAll();
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

