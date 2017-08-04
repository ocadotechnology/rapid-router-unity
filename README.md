[![Build Status](https://travis-ci.org/ocadotechnology/rapid-router-unity.svg?branch=master)](https://travis-ci.org/ocadotechnology/rapid-router-unity)

# Rapid Router in Unity
An implementation of the Rapid Router game in Unity for devices

## Building the Project
The project is currently built using TravisCI, and can be found here: [Ocado Technology Rapid Router Unity build](https://travis-ci.org/ocadotechnology/rapid-router-unity). A build is triggered for every push to a branch, each pull request, and a Cron job is setup to run every weekly. The current state of the master branch can be seen at the top of this ReadMe.

### Build and Pull Requests
Before a pull request can be merged, the code on the source branch must pass two checks - the Travis push and Travis PR builds. These are automatically triggered on those specific actions, and will be cancelled if new changes are made to the branch and/or PR in place of the new build that will be created.

If either of these checks fail (or in the case of a push build being completed before the PR is opened, just the PR check) then the pull request cannot be accepted to master. The details link to the right of the check status can be used to directly take you to the Travis build that failed, with the build logs output for diagnosis.
