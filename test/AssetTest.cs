using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using System.Linq;
using Xunit;


public class AssetTest
{
    const string chainId = "6539EC234FC62BE2B3F6C8B391FC4BBAA75455DAEF1F32CD0D3674BADEE8F19F";
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

        Asset[] assets = await Asset.GetByName(assetName, blockchain);
        Assert.Equal(1, assets.Length);
        Assert.Contains(asset.Name, assets[0].Name);
    }

    // should be returned when queried by id
    [Fact(Skip = "Working")]
    public async void AssetTestRun3()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

        var assetName = TestUtil.GenerateAssetName();
        var testChainId = TestUtil.GenerateId();
        var assetId = Chromia.Postchain.Client.GTV.Gtv.Hash(new List<dynamic>(){assetName, testChainId}.ToArray());
        
        Asset asset = await Asset.Register(
            assetName,
            testChainId,
            blockchain
        );

        Asset expectedAsset = await Asset.GetById(assetId, blockchain);

        Assert.Equal(assetName, expectedAsset.Name);
        Assert.Equal(assetId, expectedAsset.GetId());
        Assert.Equal(Util.ByteArrayToString(testChainId), Util.ByteArrayToString(expectedAsset.ChainId));
    }

    // should return all the assets registered
    [Fact(Skip = "Working")]
    public async void AssetTestRun4()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        
        var asset1 = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);
        var asset2 = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);
        var asset3 = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        Asset[] expectedAsset = await Asset.GetAssets(blockchain);
        var assetNames = expectedAsset.Select(elem => elem.Name);
        Assert.Contains(asset1.Name, assetNames);
        Assert.Contains(asset2.Name, assetNames);
        Assert.Contains(asset3.Name, assetNames);
    }
}