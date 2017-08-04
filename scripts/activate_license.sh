
#! /bin/sh

log_file=$(pwd)/build/unity-mac.log

echo "Create Certificate folder"
mkdir ~/Library/Unity
mkdir ~/Library/Unity/Certificates

cp CACerts.pem ~/Library/Unity/Certificates/

echo "Activating license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
-quit -batchmode \
-username ${UNITY_USERNAME} -password ${UNITY_PASSWORD} \
-logfile -nographics
