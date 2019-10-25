using System;
using Chromia.Postchain.Ft3;
using Xunit;


public class AssetTest
{
    const string chainId = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
    const string nodeUrl = "http://localhost:7740";

    // should be successfully registered
    [Fact(Skip = "Working")]
    public async void AssetTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

        Asset asset = await Asset.Register(
            TestUtil.GenerateAssetName(),
            TestUtil.GenerateId(),
            blockchain
        );

        Assert.NotNull(asset);
    }

    // should be returned when queried by name
    [Fact(Skip = "Working")]
    public async void AssetTestRun2()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

        var assetName = TestUtil.GenerateAssetName();
        Asset asset = await Asset.Register(
            assetName,
            TestUtil.GenerateId(),
            blockchain
        );

        var assets = await Asset.GetByName(assetName, blockchain);
        Console.WriteLine("Asset Name: " + assets[0].Name);

        Assert.Equal(1, assets.Length);
        Assert.Equal(asset.GetId(), assets[0].GetId());
    }
}