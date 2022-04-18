using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestFileGetter
{
    [Test]
    public void TestConstructor()
    {
        //Given a specific directory
        //when creating a new fileGetter
        //then that directory is created and the storage path is updated inside the filegetter.
        var storagePath = Application.persistentDataPath + "/StreamingAssets/";
        string directory = "directory";
        var path = Path.Combine(storagePath, directory);
        var sut = new FileGetter(directory);
        Assert.IsTrue(Directory.Exists(sut.StoragePath));
        Assert.AreEqual(path, sut.StoragePath);
    }
    [Test]
    public void TestGetFileLocation()
    {
        //Given a filegetter, and a filename that includes bad-for-paths characters.
        //when getfilelocation is called
        //Then a path to the file is returned without any of those bad-for-paths characters.
        string unFormattedFileName = "http://fileName";
        var sut = new FileGetter();
        string expectedfileLocation = Path.Combine(Application.persistentDataPath + sut.StreamingAssetsPath, "fileName");
        var fileLocation = sut.GetFileLocation(unFormattedFileName);
        Assert.AreEqual(expectedfileLocation, fileLocation);
    }
    [Test]
    public void TestFileExists_fails()
    {
        //Given a filegetter, and a file that doesn't exists on the drive.
        //When file exists is called
        //Then file isn't found and fileexists returns false.
        string uniqueFileName = "http://fileName"+Random.Range(0, int.MaxValue);
        var sut = new FileGetter();
        Assert.IsFalse(sut.FileExists(uniqueFileName));
    }

    [Test]
    public void TestFileExists_success()
    {
        //Given a filegetter, and a file that exists on the drive called "testExists.txt"
        //When file exists is called
        //Then file is found and fileexists returns true.
        string uniqueFileName = "testExists.txt";
        string uniqueFileContent = "test";
        var sut = new FileGetter();
        File.WriteAllText(sut.GetFileLocation(uniqueFileName), uniqueFileContent);
        Assert.IsTrue(sut.FileExists(uniqueFileName));
    }
}
