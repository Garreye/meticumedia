// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    public class MovieFolderScan : Scan
    {
        public MovieFolderScan(bool background)
            : base(background)
        {
        }

        /// <summary>
        /// Get unknown files (files that are not in a content folder) from list of movie root folders .
        /// </summary>
        /// <param name="folders">Root folders to look for files in</param>
        /// <returns>List of unknown files</returns>
        private List<OrgPath> GetUnknownFiles(List<ContentRootFolder> folders)
        {
            // Initialize file list
            List<OrgPath> files = new List<OrgPath>();

            // Get files from each folder
            foreach (ContentRootFolder folder in folders)
                GetUnknownFiles(folder, files);

            // Return files
            return files;
        }

        /// <summary>
        /// Recursively gets unknown files (files that are not in a content folder) from a movie root folder and its children.
        /// </summary>
        /// <param name="folder">Root folders to look for files in</param>
        /// <param name="files">List of files found to add to</param>
        private void GetUnknownFiles(ContentRootFolder folder, List<OrgPath> files)
        {
            // Only get files from folders that allow organization
            if (folder.AllowOrganizing)
            {
                string[] fileList = Directory.GetFiles(folder.FullPath);
                foreach (string file in fileList)
                    files.Add(new OrgPath(file, true, folder.AllowOrganizing, folder, null));
            }

            // Recursion on sub-content folders
            foreach (ContentRootFolder subfolder in folder.ChildFolders)
                GetUnknownFiles(subfolder, files);
        }

        /// <summary>
        /// Get files from  that are known to belong to a movie (in content folder) from list of root folders
        /// </summary>
        /// <param name="folders">Root folders to look for files in</param>
        /// <returns>List of known files</returns>
        private List<OrgPath> GetKnownFiles(List<ContentRootFolder> folders)
        {
            // Initialize file list
            List<OrgPath> files = new List<OrgPath>();

            // Get files from each folder
            foreach (ContentRootFolder folder in folders)
                GetKnownFiles(folder, folder.FullPath, files, 0, null);

            // Return files
            return files;
        }

        /// <summary>
        /// Recursively gets known files (files that are in a content folder) from a movie root folder and its children.
        /// </summary>
        /// <param name="folder">Root folders to look for files in</param>
        /// <param name="folderPath">Current folder path</param>
        /// <param name="files">List of files to add to</param>
        /// <param name="depth">Current depth from root folder</param>
        /// <param name="movie">Movie current path belongs to</param>
        private void GetKnownFiles(ContentRootFolder folder, string folderPath, List<OrgPath> files, int depth, Movie movie)
        {
            // Match to movie
            if (depth == 1)
            {
                foreach (Movie mov in Organization.Movies)
                    if (mov.Path == folderPath)
                    {
                        movie = mov;
                        break;
                    }
            }

            // Only get files from folders that allow organization
            if (folder.AllowOrganizing)
            {
                // Get files only from non-content sub-folders
                if (depth > 0)
                {
                    string[] fileList = Directory.GetFiles(folderPath);
                    foreach (string file in fileList)
                        files.Add(new OrgPath(file, true, folder.AllowOrganizing, folder, movie));
                }

                // Recursion on sub folders
                string[] subDirs = Directory.GetDirectories(folderPath);
                foreach (string subDir in subDirs)
                {
                    ContentRootFolder subDirContent = null;
                    foreach (ContentRootFolder subfolder in folder.ChildFolders)
                        if (subfolder.FullPath == subDir)
                        {
                            subDirContent = subfolder;
                            break;
                        }

                    if (subDirContent != null)
                        GetKnownFiles(subDirContent, subDirContent.FullPath, files, 0, movie);
                    else
                        GetKnownFiles(folder, subDir, files, depth + 1, movie);
                }
            }

        }
        /// <summary>
        /// Performs a scan on set of movie folders.
        /// </summary>
        /// <param name="folders">Movie folders to perform scan on</param>
        /// <param name="queuedItems">Items already in the queue</param>
        /// <returns>List of organization items resulting from scan</returns>
        public List<OrgItem> RunScan(List<ContentRootFolder> folders, List<OrgItem> queuedItems)
        {
            // Set scanning flag
            scanRunning = true;

            // Go thorough each folder and create actions for all files
            List<OrgItem> scanResults = new List<OrgItem>();
            RunScan(folders, scanResults, queuedItems);

            // Clear scanning and cancel flag
            scanRunning = false;
            cancelRequested = false;

            // Return results
            return scanResults;
        }

        /// <summary>
        /// Recursively run a scan on a set of movie folders and their sub-folders
        /// </summary>
        /// <param name="folders">Movie folder to perform scan on</param>
        /// <param name="scanResults">Scan results to build on.</param>
        /// <param name="queuedItems">Items already in the queue</param>
        private void RunScan(List<ContentRootFolder> folders, List<OrgItem> scanResults, List<OrgItem> queuedItems)
        {
            // Get unknown files (files not in content folder) from root folders
            OnProgressChange(ScanProcess.FileCollect, string.Empty, 0);
            List<OrgPath> files = GetUnknownFiles(folders);

            // Initialize item numbering
            int number = 0;

            // Go through unknwon files and try to match each file to a movie
            for (int i = 0; i < files.Count; i++)
            {
                // Check for cancellation
                if (scanCanceled)
                    break;

                // Update progress
                OnProgressChange(ScanProcess.Movie, files[i].Path, (int)Math.Round((double)i / files.Count * 70));

                // Categorize the file
                FileCategory fileCat = FileHelper.CategorizeFile(files[i], files[i].Path);

                // Check that video file (tv is okay, may match incorrectly)
                if (fileCat != FileCategory.MovieVideo && fileCat != FileCategory.TvVideo && fileCat != FileCategory.Trash)
                    continue;

                // Check that file is not already in the queue
                bool alreadyQueued = false;
                foreach (OrgItem queued in queuedItems)
                    if (queued.SourcePath == files[i].Path)
                    {
                        OrgItem newItem = new OrgItem(queued);
                        newItem.Action = OrgAction.Queued;
                        newItem.Number = number++;
                        scanResults.Add(newItem);
                        alreadyQueued = true;
                        break;
                    }
                if (alreadyQueued)
                    continue;

                // Check for trash
                if (fileCat == FileCategory.Trash)
                {
                    OrgItem delItem = new OrgItem(OrgAction.Delete, files[i].Path, fileCat, files[i].OrgFolder);
                    scanResults.Add(delItem);
                    continue;
                }

                // Try to match file to movie
                string search = Path.GetFileNameWithoutExtension(files[i].Path);
                Movie searchResult;
                bool searchSucess = SearchHelper.MovieSearch.ContentMatch(search, files[i].RootFolder.FullPath, string.Empty, false, true, out searchResult, null);

                // Add closest match item
                OrgItem item = new OrgItem(OrgAction.None, files[i].Path, fileCat, files[i].OrgFolder);
                item.Number = number++;
                if (searchSucess && !string.IsNullOrEmpty(searchResult.DatabaseName))
                {
                    item.Action = OrgAction.Move;
                    item.DestinationPath = searchResult.BuildFilePath(files[i].Path);
                    searchResult.Path = searchResult.BuildFolderPath();
                    item.Movie = searchResult;
                    item.Enable = true;
                }
                scanResults.Add(item);
            }

            // Get knwon movie files (files from within content folders)
            files = GetKnownFiles(folders);

            // Go through each known file and check if renaming is needed
            for (int i = 0; i < files.Count; i++)
            {
                // Check for cancellation
                if (scanCanceled)
                    break;

                // Update progress
                OnProgressChange(ScanProcess.Movie, files[i].Path, (int)Math.Round((double)i / files.Count * 20) + 70);

                // Categorize the file
                FileCategory fileCat = FileHelper.CategorizeFile(files[i], files[i].Path);

                // Check that video file (tv is okay, may match incorrectly)
                if (fileCat != FileCategory.MovieVideo && fileCat != FileCategory.TvVideo && fileCat != FileCategory.Trash)
                    continue;

                // Check that movie is valide
                Movie movie = (Movie)files[i].Content;
                if (movie == null || string.IsNullOrEmpty(movie.DatabaseName))
                    continue;

                // Check that file is not already in the queue
                bool alreadyQueued = false;
                foreach (OrgItem queued in queuedItems)
                    if (queued.SourcePath == files[i].Path)
                    {
                        alreadyQueued = true;
                        break;
                    }
                if (alreadyQueued)
                    continue;

                // Check for trash
                if (fileCat == FileCategory.Trash)
                {
                    OrgItem delItem = new OrgItem(OrgAction.Delete, files[i].Path, fileCat, files[i].OrgFolder);
                    scanResults.Add(delItem);
                    continue;
                }

                // Check if file needs to be renamed
                string newPath = movie.BuildFilePathNoFolderChanges(files[i].Path);
                if (newPath != files[i].Path && !File.Exists(newPath))
                {
                    // Add rename to results
                    OrgItem item = new OrgItem(OrgAction.Rename, files[i].Path, FileCategory.MovieVideo, files[i].OrgFolder);
                    item.Number = number++;
                    if (Path.GetDirectoryName(newPath) != Path.GetDirectoryName(files[i].Path))
                        item.Action = OrgAction.Move;

                    item.DestinationPath = newPath;
                    item.Movie = movie;
                    item.Enable = true;
                    scanResults.Add(item);
                }
            }

            // Check if any movie folders need to be renamed!
            foreach (Movie movie in Organization.GetContentFromRootFolders(folders))
            {
                if (!string.IsNullOrEmpty(movie.DatabaseName) && movie.Path != movie.BuildFolderPath())
                {
                    OrgItem item = new OrgItem(OrgAction.Rename, movie.Path, FileCategory.Folder, movie, movie.BuildFolderPath(), null);
                    item.Enable = true;
                    item.Number = number++;
                    scanResults.Add(item);
                }
            }

            // Update progress
            OnProgressChange(ScanProcess.Movie, string.Empty, 100);
        }
    }
}
