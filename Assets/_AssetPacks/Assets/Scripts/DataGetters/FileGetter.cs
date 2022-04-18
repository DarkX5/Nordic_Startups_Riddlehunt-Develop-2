using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileGetter
{
    public string StoragePath { get; }
    public readonly string StreamingAssetsPath = "/StreamingAssets/";
    public FileGetter(string directory = "")
    {
        StoragePath = Application.persistentDataPath + StreamingAssetsPath;
        StoragePath = Path.Combine(StoragePath, directory);
        if (!Directory.Exists(StoragePath))
        {
            Directory.CreateDirectory(StoragePath);
        }
    }
    public string GetFileLocation(string fileName)
    {
        fileName = FormatFilePath(fileName);
        return Path.Combine(StoragePath, fileName);
    }
    public bool FileExists(string fileName)
    {
        fileName = FormatFilePath(fileName);
        return File.Exists(GetFileLocation(fileName));
    }

    private string FormatFilePath(string fileName)
    {
        fileName = fileName.Replace("http://", "");
        fileName = fileName.Replace("https://","");
        fileName = fileName.Replace("/", "");
        fileName = fileName.Replace(" ", "");
        fileName = fileName.Replace(@"\", "");
        fileName = fileName.Replace(@"?", "");

        return fileName;
    }
}
