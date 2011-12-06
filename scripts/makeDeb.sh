#!/bin/bash
# Script for create deb package

# VARIABLES
name=scrabble
version=0.1
homepage=http://github.com/Kedrigern/srabble

function prepareStructure {

# DEPENDENCY	
if msgfmt -V 1> /dev/null; then :
else exit 1;
fi;

# MAKE structure for dpkg-deb
mkdir -p tmp
cd tmp
mkdir -p DEBIAN
mkdir -p usr
mkdir -p usr/games
mkdir -p usr/share
mkdir -p usr/share/applications
mkdir -p usr/share/doc
mkdir -p usr/share/doc/${name}
mkdir -p usr/share/games/${name}
mkdir -p usr/share/games/${name}/dics
mkdir -p usr/share/pixmaps
mkdir -p usr/share/locale/cs/LC_MESSAGES

# DEBUG
#echo "pwd"
#pwd
#echo "ls"
#ls

# -- COPY files --
cp ../../Scrabble/bin/Release/Scrabble.exe ./usr/share/games/${name}
mv ./usr/share/games/${name}/Scrabble.exe ./usr/share/games/${name}/scrabble.exe
#to post install
#ln -s /usr/share/games/${name}/scrabble.exe /usr/games/scrabble
# -- LOCALIZATION --
if [ -f ../cs.po ]; then
msgfmt ../cs.po -o ${name}.mo
mv ${name}.mo usr/share/locale/cs/LC_MESSAGES/
fi;
# -- ICON --
if [ -f ../../Scrabble/Resources/icon.svg ]; then
cp ../../Scrabble/Resources/icon.svg usr/share/pixmaps
mv usr/share/pixmaps/icon.svg usr/share/pixmaps/${name}.svg
fi;

# -- LAUNCHER --
echo "[Desktop Entry]
Name=${name}
Comment=Clasic desktop game Scrabble.
Exec=/usr/games/scrabble
Terminal=false
Type=Application
Icon=${name}.png
Encoding=UTF-8
Categories=Game" > usr/share/applications/${name}.desktop

# -- COPYRIGHT --
 
echo "This package was debianized by Ondřej Profant <ondrej.profant@gmail.com> 

It was downloaded from ${homepage}

Copyright (C) 2011-2012 Ondřej Profant

License: GPL

This program is free software; you may redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2, or (at your option)
any later version.

This is distributed in the hope that it will be useful, but without
any warranty; without even the implied warranty of merchantability or
fitness for a particular purpose. See the GNU General Public License
for more details.

A copy of the GNU General Public License version 2 is available as
/usr/share/common-licenses/GPL-2 in the Debian GNU/Linux distribution
or at http://www.gnu.org/licenses/old-licenses/gpl-2.0.html.
You can also obtain it by writing to the Free Software Foundation, Inc.,
51 Franklin St, Fifth Floor, Boston, MA 02110-1301, USA." > usr/share/doc/${name}/copyright

# -- CHANGELOG --
echo "${name} (0.1.0) unstable; urgency=low

  * This is early alpha version, for changelog use git logs (homepage), thanks.

 -- Ondřej Profant <ondrej.profant@gmail.com>" > usr/share/doc/${name}/changelog
gzip -9 usr/share/doc/${name}/changelog

# DICs
../download-dictionary.sh

mv ./dic-*.txt ./usr/share/games/${name}/dics/


# -- MAKE md5sum --
find * -type f ! -regex '^DEBIAN/.*' -exec md5sum {} \; > DEBIAN/md5sums

# size of folder in Kb
size=`du -k --total usr | sed -n '$p' | cut -f 1`

# create control file
echo -n "Package: ${name}
Version: ${version}
Section: games
Priority: optional
Recommends:
Depends: mono-runtime (>= 2.10.5), gtk-sharp2 (>= 2.12.10)
Architecture: all
Homepage: ${homepage}
Installed-Size: " > DEBIAN/control

echo $size >> DEBIAN/control

echo "Maintainer: Ondřej Profant <ondrej.profant@gmail.com>
Description: Clasic desktop game Scrabble
 Scrabble is a word game in which players score points by forming words.
 Use GADDAG algorithm, Czech and English dictionaries are inlcuded. 
 Writen in C# (mono) and GTK# (gtk front-end)." >> DEBIAN/control

echo "#!/bin/sh
ln -s /usr/share/games/${name}/scrabble.exe /usr/games/scrabble" > DEBIAN/postinst
chmod +x DEBIAN/postinst

echo "#!/bin/sh
rm /usr/games/scrabble" > DEBIAN/postrm

chmod +x DEBIAN/postrm

cd ..

}

function prepareDEB {
echo "All is prepared. For final step we need superuser authorization:" ;
sudo chown -hR root:root tmp/*
sudo chmod -R 755 tmp/
sudo chmod 644 tmp/DEBIAN/md5sums ;
sudo chmod 644 tmp/usr/share/games/${name}/dics/*
sudo chmod 644 tmp/usr/share/applications/${name}.desktop ;
sudo chmod 644 tmp/usr/share/doc/${name}/copyright ;
sudo chmod 644 tmp/usr/share/doc/${name}/changelog.gz
sudo dpkg-deb -b tmp ${name}_${version}.deb
sudo chmod 664 ${name}_${version}.deb
}

# -- MAIN --

prepareStructure

prepareDEB

# -- CLEAN --
sudo rm -r tmp
