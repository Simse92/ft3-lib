using Chromia.Postchain.Ft3;
using Xunit;


public class AssetBalanceTest
{
    const string chainId = "6539EC234FC62BE2B3F6C8B391FC4BBAA75455DAEF1F32CD0D3674BADEE8F19F";
    const string nodeUrl = "http://localhost:7740";

    [Fact(Skip = "Working")]
    public async void AssetBalanceTestRun()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

        Asset asset1 = await Asset.Register(
            TestUtil.GenerateAssetName(),
            TestUtil.GenerateId(),
            blockchain
        );

        Asset asset2 = await Asset.Register(
            TestUtil.GenerateAssetName(),
            TestUtil.GenerateId(),
            blockchain
        );

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account = await accountBuilder.Build();

        await AssetBalance.GiveBalance(account.Id, asset1.GetId(), 10, blockchain);
        await AssetBalance.GiveBalance(account.Id, asset2.GetId(), 20, blockchain);

        var assets = await AssetBalance.GetByAccountId(account.Id, blockchain);

        Assert.Equal(2, assets.Count);
    }
}