#!/bin/bash
# Author: Ondrej Profant, 2011
# Email: ondrej.profant <> gmail.com
# Generate html documentation

cd ../Scrabble/bin/Release/

if [ -e Scrabble.exe -a -e Scrabble.xml ]; then :
else
	echo "FAIL: There aren't necesary files.";
	exit 1;
fi;

# generate doc
monodocer -importslashdoc:Scrabble.xml -assembly:Scrabble.exe -path:doc -pretty;

if [ -d doc ]; then :
else
        echo "FAIL: monodocer don't make doc directory.";
        exit 1;
fi;

monodocs2html -source:doc/ -dest:htmldoc;

if [ -d '"htmldoc"' ]; then :
else
        echo "FAIL: monodocs2html don't make htmldoc directory.";
        exit 1;
fi;


mv '"htmldoc"' htmldoc
rm -rf ../../../DOC-CS/htmldoc/
mv -t ../../../DOC-CS/ htmldoc/ 

# clean
rm -rf htmldoc/
rm -rf doc/
