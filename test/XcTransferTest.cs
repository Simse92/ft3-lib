using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using Xunit;

public class XcTransferTest
{
    const string chainId = "61A42DF2FDED147AFBF3B14DCD6F34F9F1747B60C6EF248F4ECCCF5427A73041";
    const string nodeUrl = "http://localhost:7740";

    // Cross-chain transfer
    [Fact(Skip = "Working")]
    public async void XcTransferTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        var destinationChainId = TestUtil.GenerateId();
        var destinationAccountId = TestUtil.GenerateId();
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 100);
        accountBuilder.WithPoints(1);
        Account account = await accountBuilder.Build();

        await account.XcTransfer(destinationChainId, destinationAccountId, asset.GetId(), 10);

        AssetBalance accountBalance = await AssetBalance.GetByAccountAndAssetId(account.Id, asset.GetId(), blockchain);
        AssetBalance chainBalance = await AssetBalance.GetByAccountAndAssetId(
            TestUtil.BlockchainAccountId(destinationChainId),
            asset.GetId(),
            blockchain
        );

        Assert.Equal(90, accountBalance.Amount);
        Assert.Equal(10, chainBalance.Amount);
    }   
}