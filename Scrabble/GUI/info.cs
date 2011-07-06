using System;
using Gtk;
using Scrabble.Game;

namespace Scrabble.GUI
{
	public class Info : Gtk.HBox
	{	
		Gtk.Label turn;
		Gtk.Frame pla;
		Gtk.Frame score;
		Gtk.Table scoresTable;
		Gtk.Label[] scoreValues;
		
		Scrabble.Game.Game game;
		
		public Info (Game.Game g)
		{
			pla = new Gtk.Frame("Na tahu");
			pla.BorderWidth = 5;
			score = new Gtk.Frame("Score");
			score.BorderWidth = 5;
			game = g;
			
			this.PackStart( pla );
			this.PackEnd( score );
			
			scoresTable = new Gtk.Table( (uint) g.players.Length, 2, true );
			scoreValues = new Gtk.Label[g.players.Length*2];
			for( int i=0; i < game.players.Length; i++) {
				scoreValues[ i ] = new Label( game.players[i].Name );
				scoreValues[ i+game.players.Length ] = new Label( "0" );
				scoresTable.Attach( scoreValues[i], 0 , 1, (uint) i, (uint) i+1);
				scoresTable.Attach( scoreValues[i+g.players.Length] , 1, 2, (uint) i, (uint) i+1 );
			}
			scoresTable.BorderWidth = 3;
			score.Add( scoresTable );
			
			turn = new Label();
			pla.Add(turn);
			
			this.ShowAll();
		}
		
		public void Change(string name, Scrabble.Player.Player[] players) {
			turn.Text = name;	
			scoresTable.HideAll();
			for( int i=0; i < game.players.Length; i++) {
				scoreValues[ i ] = new Label( game.players[i].Name );
				scoreValues[ i+game.players.Length ].Text = string.Format( "{0}", game.players[i].Score );
#if DEBUG
				Console.WriteLine("Hrac {0} má skore: {1}", game.players[i].Name, game.players[i].Score);
#endif
			}
			scoresTable.ShowAll();
			/*
			for( int i=game.players.Length; i < scoreValues.Length; i++) {
				scoreValues[i].Hide();
				scoreValues[i].Text = game.players[i].Score.ToString();
				scoreValues[i].Show();
			}*/
		}
	}
}

