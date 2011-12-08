#!/bin/bash
# Author: Ondřej Profant, 2011
# Email: odnrej.profant <> gmail.com
# Licence: GPL
# Install dependency programs at Debian-like systems for gScrabble hosted at: http://github.com/Kedrigern/scrabble

if [ `id -u` -ne 0 ]; then
	echo Run as root.
	exit 1;
fi;

echo Install:
echo :: mono-comlete
apt-get install mono-complete
echo :: monodevelop
apt-get install monodevelop
echo :: git:
echo version manager
apt-get install git
echo :: lintian:
echo for checking deb packages
apt-get install lintian
echo :: msgfmt:
echo generate PO files (localization)
apt-get install msgfmt
echo :: dpkg-deb:
apt-get install dpkg-deb
echo :: cstocs:
echo for right encoding in czech dictionary
apt-get install cstocs
