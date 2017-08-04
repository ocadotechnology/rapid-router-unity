[![Build Status](https://travis-ci.org/ocadotechnology/rapid-router-unity.svg?branch=master)](https://travis-ci.org/ocadotechnology/rapid-router-unity)

# Rapid Router in Unity
An implementation of the Rapid Router game in Unity for devices

## Continous Integration with Travis
The project is currently built using TravisCI, and can be found here: [Ocado Technology Rapid Router Unity build](https://travis-ci.org/ocadotechnology/rapid-router-unity). A build is triggered for every push to a branch, each pull request, and a Cron job is setup to run every weekly. The current state of the master branch can be seen at the top of this ReadMe.

### Build and Pull Requests
Before a pull request can be merged, the code on the source branch must pass two checks - the Travis push and Travis PR builds. These are automatically triggered on those specific actions, and will be cancelled if new changes are made to the branch and/or PR in place of the new build that will be created.

If either of these checks fail (or in the case of a push build being completed before the PR is opened, just the PR check) then the pull request cannot be accepted to master. The details link to the right of the check status can be used to directly take you to the Travis build that failed, with the build logs output for diagnosis.

### Updating Unity for Travis
The version of Unity specified for install to Travis to use for the build is supplied in the [install\_unity.sh](https://github.com/ocadotechnology/rapid-router-unity/blob/TravisCIDeploy/scripts/install_unity.sh) script. This comes paired with an associated hash code.

    HASH=472613c02cf7
    VERSION=2017.1.0f3

When updating the version of Unity, you will need to update the hash to be the corresponding code for the version:
* Go to the [Unity Archive](https://unity3d.com/get-unity/download/archive)
* Open the Inspector
* Select the Network tab
* Select the version of Unity you want, and download the Mac Unity Editor
* Check the request URL for the entry that appears in the Network tab of the Inspector
    * The link will be of the form:
    `http://netstorage.unity3d.com/unity/<HASH>/MacEditorInstaller/Unity-<VERSION>.pkg?_ga=XXX`
* Copy the hash code from the URL and replace the `HASH` value in the script, along with the version of Unity to be used
