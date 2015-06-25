_meticumedia_ is a windows application which automates TV and movie file organization:
  * Rename TV episode files to user-defined format (with handling for double episodes)
  * Rename movie files to user-defined format
  * Organize TV/movie folder structure
  * Check for missing TV episodes
  * Move files from your download folder

# Installation #
Latest version is available in [Releases](https://github.com/Garreye/meticumedia/releases/).

See the [getting started guide](https://github.com/Garreye/meticumedia/wiki/GettingStarted) for basics on using _meticumedia_.

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

More info on [interface page](https://github.com/Garreye/meticumedia/wiki/Interface)

# Development #
_meticumedia_ is written in C# and developed in Visual Studio Express 2012. If you wish to contribute to the project please contact me and we can work out how you can help.
