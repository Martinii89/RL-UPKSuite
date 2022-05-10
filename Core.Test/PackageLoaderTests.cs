using System;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization.RocketLeague;
using Core.Test.TestUtilities;
using Core.Types;
using Core.Utility;
using FluentAssertions;
using Xunit;

namespace Core.Tests;

public class PackageLoaderTests
{
    [Theory]
    [InlineData("CustomGame")]
    [InlineData("Engine")]
    [InlineData("GameFramework")]
    [InlineData("ProjectX")]
    [InlineData("AKAudio")]
    [InlineData("TAGame")]
    public void LoadPackageTest_FullyLoad_AllObjectsShouldBeCreated(string packageName)
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var options = new PackageCacheOptions(serializer) { SearchPaths = { @"TestData/UDK/" }, GraphLinkPackages = false };
        var packageCache = new PackageCache(options);
        var loader = new PackageLoader(serializer, packageCache, new NeverUnpackUnpacker());

        // Act
        loader.LoadPackage($"TestData/UDK/{packageName}.u", packageName);
        var pckg = loader.GetPackage(packageName);
        // Assert 
        pckg.ExportTable.Should().AllSatisfy(x => { x.Object.Should().NotBeNull(); });
        pckg.ImportTable.Should().AllSatisfy(x => { x.ImportedObject.Should().NotBeNull(); });
    }

    [Fact]
    public void LoadPackageTest_FullyLoad_CompressedPackage_FailsWithClassMissingCauseNoRelatedPackageInTestDataFolder()
    {
        // Arrange
        var fileSummarySerializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);
        var unpacker = new PackageUnpacker(fileSummarySerializer, new DecryptionProvider("keys.txt"));
        var packageSerializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage), RocketLeagueBase.FileVersion);
        var options = new PackageCacheOptions(packageSerializer) { SearchPaths = { @"TestData/" }, GraphLinkPackages = false, PackageUnpacker = unpacker };
        var packageCache = new PackageCache(options);
        var loader = new PackageLoader(packageSerializer, packageCache, unpacker);

        // Act
        var action = () => loader.LoadPackage("TestData\\RocketPass_Premium_T_SF.upk", "RocketPass_Premium_T_SF");
        var pckg = loader.GetPackage("RocketPass_Premium_T_SF");
        // Assert 
        action.Should().ThrowExactly<InvalidOperationException>().WithMessage("Make sure the class is resolved before the object that needs it");
    }

    [Fact]
    public void LoadPackageTest_FullyLoad_CompressedPackageTAGAMESANITYCHECK()
    {
        // Arrange
        var fileSummarySerializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);
        var unpacker = new PackageUnpacker(fileSummarySerializer, new DecryptionProvider("keys.txt"));
        var packageSerializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage), RocketLeagueBase.FileVersion);
        var options = new PackageCacheOptions(packageSerializer)
        {
            SearchPaths = { @"D:\SteamLibrary\steamapps\common\rocketleague\TAGame\CookedPCConsole" }, GraphLinkPackages = false, PackageUnpacker = unpacker
        };
        var packageCache = new PackageCache(options);
        var loader = new PackageLoader(packageSerializer, packageCache, unpacker);

        // Act
        loader.LoadPackage("D:\\SteamLibrary\\steamapps\\common\\rocketleague\\TAGame\\CookedPCConsole\\TAGame.upk", "TAGame");
        var pckg = loader.GetPackage("TAGame");
        // Assert 
        pckg.ExportTable.Should().AllSatisfy(x => { x.Object.Should().NotBeNull(); });
        pckg.ImportTable.Should().AllSatisfy(x => { x.ImportedObject.Should().NotBeNull(); });
    }
}