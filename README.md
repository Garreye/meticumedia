# Downloads #
Downloads have been moved to [Google drive](https://drive.google.com/folderview?id=0B1Haz1DPaT-2NmM3SWJaVXVsbVk&usp=sharing). (Google code no longer supports downloading.)

# Overview #
_meticumedia_ is a windows application which automates TV and movie file organization:
  * Rename TV episode files to user-defined format (with handling for double episodes)
  * Rename movie files to user-defined format
  * Manage TV/movie folder structure
  * Check for missing TV episodes
  * Move files from your download folder

A [getting started guide](http://code.google.com/p/meticumedia/wiki/GettingStarted) is include in the wiki.

Please visit the [forums](http://meticumedia.proboards.com) and give me feedback!

# Settings #
The settings in _meticumedia_ allow the user to specify:
  * TV and movie folders directories
  * The format for TV episode file names
  * The format for movie file names
  * Folders to scan for new TV/movie files (e.g. downloads folder)
  * File types used in scanning

# Interface #
_meticumedia_ has the following tabs in its interface:
  * TV Shows - displays the TV shows found in TV folders and episodes for selected show
  * TV Schedule - displays a list of recent or upcoming TV episodes (from shows in TV folders)
  * Movies - displays the movies found in movie folders
  * Scan - allows the user to perform scans of the TV folders, movie folder, or scan folder and creates a list of organization actions that can be performed
  * Queue - queue of current organization action being processed (runs asynchronously, so that interface is not busy while items are being process)
  * Log - list of previously performed organization actions

![http://wiki.meticumedia.googlecode.com/hg/Images/Overview.jpg](http://wiki.meticumedia.googlecode.com/hg/Images/Overview.jpg)

More info on [interface wiki page](http://code.google.com/p/meticumedia/wiki/Interface)

# Development #
_meticumedia_ is written in C# and developed in Visual Studio Express 2012. If you wish to contribute to the project please send me an email and we can work out how you can help.