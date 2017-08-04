#! /bin/sh

project=$1
log_file=$(pwd)/build/unity-mac.log

error_code=0

echo "Building $project for iOS Platform."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics \
  -logFile "$log_file" \
  -returnlicense \
  -quit \
  -executeMethod "BuildBinaries.BuildForIOS"
if [ $? = 0 ] ; then
  echo "Building iOS binaries completed successfully."
  echo "Zipping binaries..."
  zip -r iOSBuild.zip . -i "$pwd/build/iOSBuild"
  error_code=0
else
  echo "Building iOS binaries failed. Exited with $?."
  error_code=1
fi

echo 'Build logs:'
cat $log_file

echo "Finishing with code $error_code"
exit $error_code
