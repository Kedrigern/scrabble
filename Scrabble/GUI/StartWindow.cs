//  
//  StartWindow.cs
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
using System.IO;
using System.Threading;
using Gtk;

namespace Scrabble.GUI
{
	public partial class StartWindow: Gtk.Window
	{	
		// Dictionary loading (in own thread)
		static object dicLoc = new System.Object();
		Scrabble.Lexicon.GADDAG dic;
		Scrabble.Player.Player[] players;
		int numberOfPlayers;
		System.Threading.Thread tdic;
		
		// TOP
		Gtk.VBox mainVbox;
	
		// Upper Line
		Gtk.HBox upperHbox;
		Gtk.Label l1;
		Gtk.Label l2;
		Gtk.SpinButton entryNum;
		Gtk.CheckButton client;
	
		// Bottom Table
		Gtk.Table table;
		Gtk.Label[] labels;
		Gtk.Entry[] entryes;
		Gtk.CheckButton[] CPUchecks;
		Gtk.CheckButton[] MPchecks;
		Gtk.Entry[] IPs;
		Gtk.Label ipLabel;
		Gtk.Button ok;

		// infoText
		Gtk.Label infoText;
	
		public StartWindow (): base (Gtk.WindowType.Toplevel)
		{
			this.Title = "Scrabble - Základní nastavení";
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			numberOfPlayers = 2;
		
			// Own thread for loading dictionary
			tdic = new Thread( LoadDictionary );
			tdic.Start();
			
			// infoText
			infoText = new Gtk.Label ("Základní nastavení hry.\n" +
			"Nastavte prosím počet hráčů, jejich jména a určete zda za ně bude hrát umělá inteligence. " +
			"Také můžete nastavit, kteří hráči se připojují vzdáleně.");
			infoText.Wrap = true;
		
			// upperline
			upperHbox = new HBox (false, 5);
			l1 = new Label ("Počet hráčů");
			upperHbox.PackStart (l1);
			l2 = new Label (", nebo");
			entryNum = new SpinButton (2, 5, 1);
			entryNum.Changed += OnNumberOfPlayerChange;
			client = new CheckButton ();
			client.Clicked += IamClient;
			client.Label = "Jsem client";
			upperHbox.Add (entryNum);
			upperHbox.Add (l2);
			upperHbox.PackEnd (client);
			upperHbox.WidthRequest = 20;
				
			// table
			table = new Gtk.Table (5, 7, false);
			table.Attach (new Gtk.Label ("Jméno hráče:"), 1, 2, 0, 1);
			table.Attach (new Gtk.Label ("CPU"), 2, 3, 0, 1);
			table.Attach (new Gtk.Label ("Network"), 3, 4, 0, 1);
			ipLabel = new Gtk.Label ("IP");
			table.Attach (ipLabel, 4, 5, 0, 1); 	
		
			labels = new Gtk.Label[5];
			entryes = new Gtk.Entry[5];
			CPUchecks = new Gtk.CheckButton[5];
			MPchecks = new Gtk.CheckButton[5];
			IPs = new Gtk.Entry[5];
			for (int i=0; i < 5; i++) {
				labels [i] = new Gtk.Label ((i + 1).ToString ());
				labels [i].Name = string.Format ("l {0}", i);
				table.Attach (labels [i], 0, 1, (uint)i + 1, (uint)i + 2);
				entryes [i] = new Gtk.Entry (12);
				entryes [i].Text = "Hráč " + (i+1).ToString();
				table.Attach (entryes [i], 1, 2, (uint)i + 1, (uint)i + 2);
				CPUchecks [i] = new Gtk.CheckButton ();
				CPUchecks [i].Name = string.Format ("c {0}", i);
				((Gtk.CheckButton)CPUchecks [i]).Clicked += OnCpuChange;
				table.Attach (CPUchecks [i], 2, 3, (uint)i + 1, (uint)i + 2);
				MPchecks [i] = new Gtk.CheckButton ();
				MPchecks [i].Name = string.Format ("n {0}", i);
				((Gtk.CheckButton)MPchecks [i]).Clicked += OnNetChange;
				table.Attach (MPchecks [i], 3, 4, (uint)i + 1, (uint)i + 2);
				IPs [i] = new Gtk.Entry (15);
				IPs [i].Text = "192.168.0.X";
				IPs [i].WidthChars = 15;
				IPs [i].Sensitive = false;
				table.Attach (IPs [i], 4, 5, (uint)i + 1, (uint)i + 2);
			}
		
			ok = new Button ("Hotovo");
			ok.Clicked += Done;
			table.Attach (ok, 0, 5, 6, 7);

			// Main vbox
			mainVbox = new Gtk.VBox (false, 5);
		
			mainVbox.PackStart (infoText);
			mainVbox.Add (upperHbox);
			mainVbox.Spacing = 5;
			mainVbox.PackEnd (table);
		
			this.Add (mainVbox);
			mainVbox.ShowAll ();	
		
			for (int i = 2; i < numberOfPlayers+3; i++) {
				labels [i].Hide ();
				entryes [i].Hide ();
				CPUchecks [i].Hide ();
				MPchecks [i].Hide ();
				IPs [i].Hide ();
			}
			ipLabel.Hide ();
		
			foreach (Gtk.Entry e in IPs)
				e.Hide ();
		}
	
		protected void OnNumberOfPlayerChange (object sender, EventArgs e)
		{
			int newVal = ((Gtk.SpinButton)sender).ValueAsInt;
			if (newVal >= numberOfPlayers) {
				for (int i = numberOfPlayers+2; i<newVal+2; i++) {
					labels [i - 2].Show ();
					entryes [i - 2].Show ();
					CPUchecks [i - 2].Show ();
					MPchecks [i - 2].Show ();	
					if (MPchecks [i - 2].Active)
						IPs [i - 2].Show ();
				}
			} else {
				for (int i = newVal; i< numberOfPlayers; i++) {
					labels [i].Hide ();
					entryes [i].Hide ();
					CPUchecks [i].Hide ();
					MPchecks [i].Hide ();
					IPs [i].Hide ();

				}
			}
			numberOfPlayers = newVal;		
		}
	
		protected void OnCpuChange (object sender, EventArgs e)
		{
			int i = int.Parse (((Gtk.CheckButton)sender).Name.Split (new char[] {' '}, StringSplitOptions.RemoveEmptyEntries) [1]);
		
			if (((Gtk.CheckButton)sender).Active) {
				if (((CheckButton)MPchecks [i]).Active) {
					((CheckButton)MPchecks [i]).Active = false;
					IPs [i].Sensitive = false;
				}
				((CheckButton)MPchecks [i]).Sensitive = false;
			} else {
				((CheckButton)MPchecks [i]).Sensitive = true;	
			}
		}
	
		protected void OnNetChange (object sender, EventArgs e)
		{
			int i = int.Parse (((Gtk.CheckButton)sender).Name.Split (new char[] {' '}, StringSplitOptions.RemoveEmptyEntries) [1]);
		
			if (((Gtk.CheckButton)sender).Active) {
				ok.Show ();
				ipLabel.Show ();
				IPs [i].Show ();
				IPs [i].Sensitive = true;
				ipLabel.Show ();
				CPUchecks [i].Active = false;
				CPUchecks [i].Sensitive = false;
			} else {
				CPUchecks [i].Sensitive = true;
			}
		}
	
		protected void IamClient (object sender,EventArgs e)
		{
			if (((Gtk.CheckButton)sender).Active) {
				client.Sensitive = false;
				entryNum.Sensitive = false;
				ipLabel.Show ();
				IPs [0].Show ();
				IPs [0].Sensitive = true;
				MPchecks [0].Active = true;
				MPchecks [0].Sensitive = false;
				CPUchecks [0].Active = false;
				CPUchecks [0].Sensitive = false;
				for (int i=1; i < numberOfPlayers; i++) {
					labels [i].Hide ();
					entryes [i].Hide ();
					CPUchecks [i].Hide ();
					MPchecks [i].Hide ();
				}
			} else {
				//TODO: Deactivation of button (all implementation)
			}
		}
		
		protected void LoadDictionary() {
			lock( dicLoc ) {				
				dic = new Scrabble.Lexicon.GADDAG();
				if( File.Exists( "./dic.txt" ) ) {
					StreamReader sr = new StreamReader ( "./dic.txt" );
					dic = new Scrabble.Lexicon.GADDAG(sr);	

				} 
			}
#if DEBUG
			Console.WriteLine("[INFO] Slovník obsahuje {0} slov.", dic.WordCount);
#endif
		}
	
		protected void Done (object sender, EventArgs e)
		{		
			this.HideAll ();
			
			if (client.Active) {
				players = new Player.Player[1];
				players[0] = new Scrabble.Player.Player( entryes[0].Text);
			} else {
				players = new Player.Player[ numberOfPlayers ];
				for( int i=0; i < numberOfPlayers; i++) {
					if( CPUchecks[i].Active ) {
						players[i] = new Scrabble.Player.ComputerPlayer( entryes[i].Text, null );
						continue;
					}
					if( MPchecks[i].Active ) {
						players[i] = new Scrabble.Player.NetworkPlayer( entryes[i].Text, IPs[i].Text );
						continue;
					}
					players[i] = new Scrabble.Player.Player( entryes[i].Text );	
				}
			}
						
			Scrabble.Game.InitialConfig.players = this.players;
			
			tdic.Join();
			lock( dicLoc ) {
				Scrabble.Game.InitialConfig.dictionary = this.dic;
			}			
			
			Scrabble.Game.InitialConfig.allDone = true;
#if DEBUG
			Console.WriteLine("[INFO] Nastavení parametrů dokončeno.");
#endif
			
			this.Destroy ();
			this.Dispose ();						
			Gtk.Application.Quit();
		}
	
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
	}

}
