# CONTENT
1. install
1. dictionary
1. compile
1. licence
1. links

# INSTALL
Install isn't required. Just click at scrabble.exe, or write to terminal: `mono scrabble.exe`. Required is Mono 2.10 + Gtk sharp 2+.

# DICTIONARY
In dictionary scripts is `download-dictionary.sh`, you can use it for download dictionary in required format.

You need some dictionary file, for Czech I recommend [blex-klasik](http://scrabble.hrejsi.cz/pravidla/blex.htm)
Use it is realy simple:
1. download dictionary to text file
1. delete header and footer (in text file is permitted only words separate by comma and space)
1. rename to:	`dic-cs.txt`
1. put in same folder as Scrabble.exe , or to `/.scrabble/`

# COMPILE
Directories structures etc. is Monodevelop project (*.sln). You can open it in Monodevelop 2.6+ and compile. C# 4 (Mono 2.10) is required.

# LICENCE
Is GPL, see LICENCE file.

# LINKS
* [Project homepage](http://kedrigern.github.com/scrabble)
* [Project documentation](http://kedrigern.github.com/scrabble/doc)
* [Author homepage](http://anilinux.org/~keddie)
