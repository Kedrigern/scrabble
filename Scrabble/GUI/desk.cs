using System;
using System.Collections.Generic;
using Gtk;
using Gdk;

namespace Scrabble.GUI
{
	public class Desk : Gtk.Table
	{
		public const uint sizeX = 15;
		public const uint sizeY = 15;
		private Stone[,] fields;
		private Scrabble.Lexicon.PlayDesk Pdesk;
		private Game.Game game;
		
		public Desk (Game.Game g) : base ( sizeX , sizeY , true )
		{
			this.fields = new Stone[ sizeX+1, sizeY+1 ];
			this.game = g;
			this.Pdesk = ((Game.Game) game).desk;
			
			// inicialization of labels 
			this.fields[0,0] = new Stone();
			this.fields[0,0].Sensitive = false;
			for( uint i = 1; i < fields.GetLength(0); i++) {
				this.fields[i,0] = new Stone();
				this.Attach( fields[i,0], i,i+1,0,1);
				this.fields[i,0].Sensitive = false;
				this.fields[i,0].Show();
				this.fields[i,0].setChar(i.ToString());
				
				this.fields[0,i] = new Stone();
				this.Attach( fields[0,i], 0,1,i,i+1);
				this.fields[0,i].Sensitive = false;
				this.fields[0,i].Show();
				this.fields[0,i].setChar(i.ToString());
			}
			
			for(uint j=1; j < fields.GetLength(1) ; j++)
				for(uint i=1; i < fields.GetLength(0) ; i++) {
					fields[i,j] = new Stone();
					this.Attach( fields[i,j] , i, i+1, j, j+1);
				}
			
			this.Restart();
		}
		
		public void SetBonus(System.Drawing.Point p, short level, bool word) {
				this.fields[p.X+1,p.Y+1].setBonus( level, word );
		}
		
		public void DisableButtons() {
			foreach(Gtk.Button b in	fields )
				b.Clicked -= PushButton;
		}
		
		public void ActiveButtons() {
			foreach(Gtk.Button b in	fields ) {
				b.Clicked += PushButton;
				b.ButtonPressEvent += PushButton;
			}
		}
		
		private void PushButton( object sender, EventArgs e) {		
			Gtk.CheckButton check = new Gtk.CheckButton("Down");
			Gtk.Entry input = new Gtk.Entry(15);
			Gtk.HBox divide = new Gtk.HBox(false, 0);
			Gtk.Button but = new Gtk.Button("OK");
			
			input.Activated += delegate {
				but.Click();
			};
			
			divide.PackStart( input );
			divide.Add( check );
			divide.PackEnd( but );
			
			Gtk.Window w = new Gtk.Window( Gtk.WindowType.Popup );
			w.SetPosition ( WindowPosition.Mouse );

			w.Add( divide );			
			
			w.BorderWidth = 0;
			w.Modal = true;
			w.CanFocus = true;
			w.ShowAll();
			
			but.Clicked += delegate {
				if( input.Text == "" ) return;
				int i, j;
				string[] name = ((Gtk.Button)sender).Name.Split (new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				i = int.Parse (name [1]);
				j = int.Parse (name [2]);
				
				w.HideAll ();
				
				
				if( this.game.GetActualPlayer().DoMove(
								new Lexicon.Move( new System.Drawing.Point(i-1,j-1), input.Text.ToUpperInvariant(), check.Active )	)
					) { 
					w.Destroy ();
					if( !Scrabble.Game.InitialConfig.client ) this.game.changePlayer();
				}

			};
				
			w.KeyPressEvent += delegate(object o, KeyPressEventArgs args) {
				switch( args.Event.Key ) {
				case Gdk.Key.Escape :	
					w.HideAll ();
					w.Dispose ();
					w.Destroy ();	
					break;
				}
			};
				
		}
		
		/// <summary>
		/// Updates the desk againts the logic desk
		/// </summary>
		/// <param name='d'>
		/// Logic desk (char[,])
		/// </param>
		public void UpdateDesk(char[,] d) {
			for(uint j=1; j <= d.GetLength(1); j++)
				for(uint i=1; i <= d.GetLength(0); i++)
					fields[i,j].setChar( d[i-1,j-1] .ToString());
		}
		
		/// <summary>
		/// Init blank desk with all bonuses 
		/// </summary>
		public void Restart() {
			/* initialization of Buttons (letters) */
			for(uint j=1; j < fields.GetLength(1) ; j++)
				for(uint i=1; i < fields.GetLength(0) ; i++) {
					fields[i,j].setChar("");
					fields[i,j].Name = "B " + i + " " + j;	// name is +1 oposite the logic desk
					fields[i,j].TooltipText = "["+i+","+j+"]";
					fields[i,j].ModifyBg( StateType.Normal, this.Style.Backgrounds[ (int) StateType.Prelight ]);
					fields[i,j].Hide();		
					fields[i,j].Show();	
					fields[i,j].Clicked += PushButton;
				}
			
			/* initialization of charBonus */
			for(int j=0; j < fields.GetLength(1)-1 ; j++)
				for(int i=0; i < fields.GetLength(0)-1 ; i++) {
					if( Pdesk.CharBonus[i,j] == 1 ) continue;
					else SetBonus(new System.Drawing.Point(i,j), Pdesk.CharBonus[i,j], false);
				}
			
			/* initialization of wordBonus */
			for(int j=0; j < fields.GetLength(1)-1 ; j++)
				for(int i=0; i < fields.GetLength(0)-1 ; i++) {
					if( Pdesk.WordBonus[i,j] == 1 ) continue;
					else SetBonus(new System.Drawing.Point(i,j), Pdesk.WordBonus[i,j], true);
				}		
		}
		
		public void UnsetMove( List<Scrabble.Lexicon.MovedStone> l ) {
			foreach( Scrabble.Lexicon.MovedStone s in l) {
				this.fields[s.i+1, s.j+1].unset();
				if( this.Pdesk.CharBonus[s.i,s.j] !=1 ) 
					this.fields[s.i+1, s.j+1].setBonus( this.Pdesk.CharBonus[s.i,s.j], false );
				if( this.Pdesk.WordBonus[s.i,s.j] !=1 ) 
					this.fields[s.i+1, s.j+1].setBonus( this.Pdesk.WordBonus[s.i,s.j], true );
				
			}
		}
	}
	
	/// <summary>
	/// Auxiliary class derived from Gtk.Button. Work with colors.
	/// </summary>
	class Stone : Gtk.Button {		
		public static readonly Color red = new Color(255,0,0);
		public static readonly Color blue = new Color(0,0,255);
		public static readonly Color green = new Color(30,150,80);
		public static readonly Color pink = new Color(240,110,110);
		public static readonly Color navy = new Color(110,110,240);
		public static readonly Color back = new Color(249,216,171);
		
		public Stone() : base() {
			this.Label = "   ";
		}
	
		public void setBonus(short s, bool wBonus ) {
			switch(s) {
			case 2:
				this.ModifyBg( StateType.Normal, wBonus ? pink : navy );
				return;
			case 3:
				this.ModifyBg( StateType.Normal, wBonus ? red : blue );
				return;
			default:				
				this.ModifyBg( StateType.Normal, green);
				return;
			};
		}
		
		public void setChar(string s) {
			if( s == "_" ) return;
			this.Label = s;
			this.ModifyBg( StateType.Normal, back );				
		}
		
		public void unset() {
			this.Label = " ";
			this.ModifyBg( StateType.Normal, this.Style.Backgrounds[ (int) StateType.Prelight ]);
		}
	}
}
