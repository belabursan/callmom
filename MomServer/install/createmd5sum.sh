#!/bin/sh
#
# Script to create md5sum
# Bela Bursan <burszan@gmail.com>
#

echo "creating md5sums..."
cd momserver_deb

# on linux: md5sum -b >../deb/DEBIAN/md5sums `find etc usr -type f`
md5 > DEBIAN/md5sums `find etc usr -type f | grep -v ".DS*"`

echo "done"
echo
more DEBIAN/md5sums

