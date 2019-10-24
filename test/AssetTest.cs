using System;
using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using Chromia.Postchain.Client;
using Xunit;


public class AssetTest
{
    const string chainId = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
    const string nodeUrl = "http://localhost:7740";

    [Fact]
    public async void AssetTestRun()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

        var assetName = TestUtil.GenerateAssetName();
        Asset asset = await Asset.Register(
            assetName,
            TestUtil.GenerateId(),
            blockchain
        );

        var assets = await Asset.GetByName(assetName, blockchain);
        Console.WriteLine(assets);
    }
}