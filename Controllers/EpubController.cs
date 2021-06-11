using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace epub_searcher.Controllers 
{
    [Route("api/[controller]")]

    [ApiController]
    public class EpubController : Controller {

        [HttpPost]
        [ResponseCache(NoStore = true, Duration = 0)]
        public ActionResult<string> SearchInEpubs([FromBody] SearchInput searchInput) {
            // check for null
            if (searchInput == null || string.IsNullOrEmpty(searchInput.SourcePath) || string.IsNullOrEmpty(searchInput.SearchParams))
                return BadRequest("Information invalid");

            try {
                Dictionary<string, bool> matchDictionary = new Dictionary<string, bool>();
                List<string> searchResult = new List<string>();
                
                // if more than one search phrase was entered, make sure to search for them separately
                var wordsToMatch = searchInput.SearchParams.Split(";");

                // get every .epub file in directory and underlying directories
                foreach (string epubFile in Directory.EnumerateFiles(searchInput.SourcePath, "*.epub",
                    SearchOption.AllDirectories))
                {
                    // set/reset dictionary to "no match" (i.e. 'false') for all search parameters
                    foreach (var word in wordsToMatch)
                        matchDictionary[word] = false;
                    
                    try
                    {
                        // copy .epub to a new .zip file
                        var zipFile = Path.ChangeExtension(epubFile, ".zip");
                        System.IO.File.Copy(epubFile, zipFile);

                        // open and read .zip file
                        ZipArchive zipArchive = ZipFile.OpenRead(zipFile);
                        foreach (ZipArchiveEntry entry in zipArchive.Entries)
                        {
                            // open every .xhtml or .html entry as stream and check contents for specified words
                            if (!entry.FullName.EndsWith(".xhtml") && !entry.FullName.EndsWith(".html")) continue;
                            using var stream = entry.Open();
                            using var reader = new StreamReader(stream);
                            var content = reader.ReadToEnd();

                            if (content == null) 
                                return BadRequest("No content available");

                            // will only return the name of the file if ALL entered words were found in 
                            // any of the epub's entries.
                            foreach (var word in wordsToMatch)
                            {
                                if (content.Contains(word))
                                    matchDictionary[word] = true;
                            }
                        }
                        
                        // only prints the book title if all words could be matched
                        if(matchDictionary.Values.All(o => o)) 
                            searchResult.Add(Path.GetFileName(epubFile));

                        // delete created .zip file if match as found
                        if(System.IO.File.Exists(zipFile))
                            System.IO.File.Delete(zipFile);
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }
                string.Join(", ", searchResult);
                return Ok(searchResult);
            }
            catch(Exception e) {
                return BadRequest(e.Message);
            }
        }
    }
}