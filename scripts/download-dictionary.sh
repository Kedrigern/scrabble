#!/bin/bash
# Author: Ondrej Profant
# Email: ondrej.profant <> gmail.com
# For gScrabble hosted at http://github.com/Kedrigern/scrabble

function usage {
	echo "USAGE:"
	echo "$0"
	echo -e "\twithout parametr - download all dictionaries"
	echo "$0 [lang code]"
	echo -e "\tdownload only choden language\n"
	echo "Example:"
	echo -e "$0 cs\n"
	echo -e "Suported language: cs, en\n"
}

function parseFromHtml {
	# sed script
	# 1. little hack: dic start with letter 'a' 
	# 2. deete html tags
	sed -n '/^a/p
s/<\/*[a-z]*>//g' $1 | sed 's/,\ /,\ \n/g' > $2
}

function langcs {
	# we need cstocs
	if cstocs -v 2> /dev/null ; 
		then :
		else
			echo "Install cstocs package."
			exit;
	fi;

	# we would not delete some random file
	if [ -f blex.htm ] ; then
		echo "blex.htm already exist, please remove";
		exit
	fi;

        # download czech dictionary "blek klasik"
        echo "Downloading czech dictionary from: http://scrabble.hrejsi.cz/pravidla/blex.htm ";
        wget -q http://scrabble.hrejsi.cz/pravidla/blex.htm ;

        # convert from cp1250 to utf8
        cstocs -i cp1250 utf8 blex.htm

        # parse from html 
        parseFromHtml blex.htm dic-cs.txt

        rm -f blex.htm
}

function langen {
	echo "Download english dictionary from: /usr/share/dict/words";
	sed 's/$/,\ /g' /usr/share/dict/words > dic-en.txt ;	
}

if [ $# -eq 0 ]; then
	langcs;
	langen;
	exit;
fi;

while [ $# -gt 0 ]; do
  case $1 in
    cs)
	langcs;
	;;
    en)
	langen;
	;;
    *help)
	usage
	exit
	;;
    *h)
	usage
	exit
	;;
  esac;
  shift 1;
done;

echo "Now is your dictionaries ready to use. ";

#DEBUG
#file dicsed 's/$/,\ /g' /usr/share/dict/words > dic-en.txt ;
