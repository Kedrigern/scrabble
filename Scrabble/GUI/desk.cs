using System;
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
		
		public Desk (Scrabble.Lexicon.PlayDesk desk) : base ( sizeX , sizeY , true )
		{
			fields = new Stone[ sizeX , sizeY ];
			Pdesk = desk;
			
			/* initialization of Buttons (letters) */
			for(uint j=0; j < fields.GetLength(1) ; j++)
				for(uint i=0; i < fields.GetLength(0) ; i++) {
					fields[i,j] = new Stone();
					this.Attach( fields[i,j] , i, i+1, j, j+1);
					fields[i,j].Name = "B " + i + " " + j;
					fields[i,j].Show();	
					fields[i,j].Clicked += PushButton;
				}
			
			/* initialization of charBonus */
			for(int j=0; j < fields.GetLength(1) ; j++)
				for(int i=0; i < fields.GetLength(0) ; i++) {
					if( desk.CharBonus[i,j] == 1 ) continue;
					else SetBonus(new System.Drawing.Point(i,j), desk.CharBonus[i,j], false);
				}
			
			/* initialization of wordBonus */
			for(int j=0; j < fields.GetLength(1) ; j++)
				for(int i=0; i < fields.GetLength(0) ; i++) {
					if( desk.WordBonus[i,j] == 1 ) continue;
					else SetBonus(new System.Drawing.Point(i,j), desk.WordBonus[i,j], true);
				}
		}
		
		public void SetBonus(System.Drawing.Point p, short level, bool word) {
				fields[p.X,p.Y].setBonus( level, word );
		}
		
		private void DisableButtons() {
			foreach(Gtk.Button b in	fields )
				b.Clicked -= PushButton;
		}
		
		private void ActiveButtons() {
			foreach(Gtk.Button b in	fields )
				b.Clicked += PushButton;
		}
		
		private void PushButton( object sender, EventArgs e) {
			Gtk.CheckButton check = new Gtk.CheckButton("Down");
			Gtk.Entry input = new Gtk.Entry(15);
			Gtk.HBox divide = new Gtk.HBox(false, 0);
			Gtk.Button but = new Gtk.Button("OK");
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
				int i, j;
				string[] name = ((Gtk.Button) sender).Name.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries );
				i = int.Parse(name[1]);
				j = int.Parse(name[2]);
				setWord( check.Active, (uint) i, (uint) j, input.Text);
				
				Console.WriteLine( 			 );
				w.HideAll();
				w.Dispose();
				w.Destroy();				
			};
		}
		
		private bool setWord(bool d, uint x, uint y, string s) {
			Console.WriteLine("Mam zapsat \"{0}\" s delkou {4} na {1},{2}, down: {3}", s, x, y, d, s.Length);
			
			Lexicon.Move m = new Lexicon.Move(new System.Drawing.Point((int)x,(int)y), s.ToUpperInvariant(), d );
			
			if( ! Pdesk.AnalyzeMove( m ) ) return false;
			
			if( ! Pdesk.Connect( m ) ) {
				Console.WriteLine( "[no] Špatné napojení" );
				return false; 
			}
			
			if( ! Pdesk.Play( m ) ) return false;
			
			if( d ) {
				for( uint j=y; j< y+s.Length; j++) {
					fields[x,j].setChar(s[(int)(j-y)].ToString().ToUpper());
				}
			} else {
				for( uint i=x; i< x+s.Length; i++)	{
					fields[i,y].setChar( s[(int)(i -x)].ToString().ToUpper() );
				}
			}
						
			return true;
		}	
	}
	
	class Stone : Gtk.Button {
		bool done;
		
		public static readonly Color red = new Color(255,0,0);
		public static readonly Color blue = new Color(0,0,255);
		public static readonly Color green = new Color(30,150,80);
		public static readonly Color pink = new Color(240,110,110);
		public static readonly Color navy = new Color(110,110,240);
		public static readonly Color back = new Color(249,216,171);
		
		public Stone() : base() {
			this.Label = "   ";
			this.done = false;
		}
	
		public void setBonus(short s, bool wBonus) {
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
			if( done ) return;
			this.Label = s;
			this.done = true;
			this.ModifyBg( StateType.Normal, back );
		}
	}
}

		/*
		 * table1.Attach ( Widget            child,
                       		int              leftAttach,	// vzdálenost levého okraje tlačítka od lev. ok. tabulky
                       		int              rightAttach,	// vzdálenost pravého okraje tlačítka od lev. ok. tabulky
                       		int              topAttach,		// vzdálenost od horního okraje tabulky
                       		int              bottomAttach,
         */