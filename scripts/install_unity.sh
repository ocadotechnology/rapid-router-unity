#! /bin/sh

BASE_URL=http://netstorage.unity3d.com/unity
HASH=472613c02cf7
VERSION=2017.1.0f3

download() {
  url="$BASE_URL/$HASH/$package"

  echo "Downloading from $url: "
  curl -o `basename "$package"` "$url"
}

install() {
  package=$1
  download "$package"

  echo "Installing "`basename "$package"`
  sudo installer -dumplog -package `basename "$package"` -target /
}

install "MacEditorInstaller/Unity-$VERSION.pkg"
