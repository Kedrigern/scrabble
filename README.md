-------
CONTENT
-------
1. install
2. dictionary
3. compile
4. licence
5. links

-------
INSTALL
-------
Install isn't required. Just click at scrabble.exe, or write to terminal:
	mono scrabble.exe
but is required Mono 2.10 + Gtk sharp 2.

	OS		WORK			NOTES
- OpenSuse 11.4 	ok
- Fedora 15 		ok		packages: mono-core , gtk-sharp2 (tested on both system)
- Ubuntu 11.04 		ok (!)		has not Mono 2.10 (only 2.6.7). Way to install 2.10: 
						http://www.integratedwebsystems.com/2011/05/install-mono-2-10-2-and-monodevelop-2-6-beta-3-on-ubuntu-or-fedora-with-a-bash-script 
- Ubuntu 11.10:		ok		There is deb package in download section
- Windows 7 32bit:	ok		download "Gtk# for .NET" from go-mono.com
- Windows 7 64bit:	ok (!)		some issue with glib (in GTK# platform), thera are special binary for Win64 (is necessary to use 32bit version)
- For other Linux, Mac OS and Windows download from: http://www.go-mono.com/mono-downloads/download.html
(!) means some issues

----------
DICTIONARY
----------
In dictionary scripts is "download-dictionary.sh", you can use it for download dictionary in required format.

You need some dictionary file, for Czech I recommend blex-klasik: http://scrabble.hrejsi.cz/pravidla/blex.htm 
Use it is realy simple:
	1) download dictionary to text file
	2) delete header and footer (in text file is permitted only words separate by comma and space)
	3) rename to:	dic-cs.txt
	4) put in same folder as Scrabble.exe , or to ~/.scrabble/

-------
COMPILE
-------
Directories structures etc. is Monodevelop project (*.sln). You can open it in Monodevelop 2.6 (or newer) and compile. C# 4 (Mono 2.10) is required.

-------
LICENCE
-------
Is GPL, see LICENCE file.

-----
LINKS
-----
Project homepage: http://kedrigern.github.com/scrabble
Project documentation: http://kedrigern.github.com/scrabble/doc
Author homepage: http://anilinux.org/~keddie
