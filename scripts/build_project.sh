#! /bin/sh

project=$1
project_path=$(pwd)/src/$project
log_file=$(pwd)/build/unity-mac.log

error_code=0

echo "Building $project for Mac OS."
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile "$log_file" \
  -projectPath "$project_path" \
  -buildOSXUniversalPlayer "$(pwd)/build/osx/$project.app" \
  -quit
if [ $? = 0 ] ; then
  echo "Building Mac OS completed successfully."
  error_code=0
else
  echo "Building Mac OS failed. Exited with $?."
  error_code=1
fi

echo 'Build logs:'
cat $log_file

echo "Finishing with code $error_code"
exit $error_code
