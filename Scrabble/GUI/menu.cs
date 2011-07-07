/* Create MenuBar:
	  * all submenu, 
	  * menuitems 
	  * connections to GTK (like icons)
 * I wrote tutorial (in Czech):
 * http://anilinux.org/~keddie/index.php?page=Csharp+GtkSharp+Menu
 */

using System;
using Gtk;

namespace Scrabble.GUI {
	
	public class MenuHover {
		
		UIManager uim;
		ActionGroup ag;
		Scrabble.GUI.ScrabbleWindow parrent;
		
		/* Popdown menu */
		Gtk.Action game;
		Gtk.Action dictionary;
		Gtk.Action settings;
		Gtk.Action help;
		
		/* Items in menu */
		Gtk.Action newG;
		Gtk.Action loadG;
		Gtk.Action saveG;
		Gtk.Action endG;
		
		Gtk.Action infoD;
		Gtk.Action checkD;	
		Gtk.Action addD;
		Gtk.Action loadD;
		
		Gtk.Action optionS;
		
		Gtk.Action about;
				
		/* Menu BAR - public, for another class (like Window) */
		public MenuBar menuBar;
		
		public MenuHover( ScrabbleWindow par ) {
			this.parrent = par;
			this.uim = new UIManager();
			this.ag = new ActionGroup("Default");
			
			this.game = new Gtk.Action("game", "Hra", null, null);
			this.dictionary = new Gtk.Action("dictionary", "Slovník", null, null);
			this.settings = new Gtk.Action("settings", "Nastavení", null, null);
			this.help = new Gtk.Action("help", "Nápověda", null, null);
			this.ag.Add( game );
			this.ag.Add( dictionary );
			this.ag.Add( settings );
			this.ag.Add( help );
			
			this.newG  = new Gtk.Action("newG", "Nová",   null, "gtk-open");
			this.newG.ShortLabel = "Nová";
			this.loadG = new Gtk.Action("loadG","Načíst", "gtk-load" , "gtk-load");
			this.saveG = new Gtk.Action("saveG","Uložit", null, "gtk-save");
			this.endG  = new Gtk.Action("endG", "Konec",  null, null);
			this.endG.Activated += (sender, e) => Application.Quit();
			this.infoD = new Gtk.Action("infoD", "Info", null, null);
			this.infoD.Activated += StaticWindows.DictionaryInfoDialog;
			this.addD = new Gtk.Action("addD", "Přidat slovo", null, null);
			this.addD.Activated += StaticWindows.AddNewWordToDic;
			this.checkD = new Gtk.Action( "checkD", "Ověř slovo", null, null);
			this.checkD.Activated += StaticWindows.CheckWordDialog;
			this.loadD = new Gtk.Action( "loadD", "Načti slovník", null, null);
			this.loadD.Activated += StaticWindows.LoadNewDictionaryDialog;
			this.optionS = new Gtk.Action("optionS", "Volby", null, null);
			this.about = new Gtk.Action("about", "O aplikaci", null, null);
			this.about.Activated += StaticWindows.AboutProgramDialog;
			this.ag.Add( newG, null);
			this.ag.Add( loadG, null);
			this.ag.Add( saveG, null);
			this.ag.Add( endG, null);
			this.ag.Add( infoD, null);
			this.ag.Add( addD , null );
			this.ag.Add( checkD, null );
			this.ag.Add( loadD, null);
			this.ag.Add( optionS, null);
			this.ag.Add( about, null);
					
					
			this.uim.InsertActionGroup( ag, 0);
			parrent.AddAccelGroup( uim.AccelGroup );
			uim.AddUiFromFile("./GUI/menu.xml");		// TODO: Nějak inteligentně to ošetřit...
			this.menuBar = (MenuBar) uim.GetWidget("/menubar");
		}
	}
}