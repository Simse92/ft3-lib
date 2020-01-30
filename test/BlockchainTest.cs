using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using System.Linq;
using Xunit;

public class BlockchainTest
{
    const string chainId = "6539EC234FC62BE2B3F6C8B391FC4BBAA75455DAEF1F32CD0D3674BADEE8F19F";
    const string nodeUrl = "http://localhost:7740";

    // should provide info
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        BlockchainInfo info = await BlockchainInfo.GetInfo(blockchain.Connection);
        Assert.Equal(info.Name, "test");
    }

    // should be able to register an account
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun2()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        
        User user = TestUser.SingleSig();
        BlockchainSession session = blockchain.NewSession(user);

        Account account = await blockchain.RegisterAccount(user.AuthDescriptor, user);
        Account foundAccount = await session.GetAccountById(account.Id);

        Assert.Equal(account.Id, foundAccount.Id);
    }

    // should return account by participant id
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun3()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        Account account = await accountBuilder.Build();
        
        Account[] foundAccount = await blockchain.GetAccountsByParticipantId(user.KeyPair.PubKey, user);

        Assert.Equal(1, foundAccount.Length);
        Assert.Equal(account.Id, foundAccount[0].Id);
    }

    // should return account by auth descriptor id
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun4()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        Account account = await accountBuilder.Build();
        
        Account[] foundAccount = await blockchain.GetAccountsByAuthDescriptorId(user.AuthDescriptor.Hash(), user);

        Assert.Equal(1, foundAccount.Length);
        Assert.Equal(account.Id, foundAccount[0].Id);
    }

    // should be able to link other chain
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun5()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        var chainId1 = TestUtil.GenerateId();
        await blockchain.LinkChain(chainId1);

        Assert.Equal(true, await blockchain.IsLinkedWithChain(chainId1));
    }

    // should be able to link multiple chains
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun6()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        var chainId1 = TestUtil.GenerateId();
        var chainId2 = TestUtil.GenerateId();

        await blockchain.LinkChain(chainId1);
        await blockchain.LinkChain(chainId2);

        List<byte[]> linkedChains = await blockchain.GetLinkedChainsIds();
        var parsedChains = linkedChains.Select(elem => Util.ByteArrayToString(elem));

        Assert.Contains(Util.ByteArrayToString(chainId1), parsedChains);
        Assert.Contains(Util.ByteArrayToString(chainId2), parsedChains);
    }

    // should return false when isLinkedWithChain is called for unknown chain id
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun7()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Assert.Equal(false, await blockchain.IsLinkedWithChain(TestUtil.GenerateId()));
    }

    // should return asset queried by id
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun8()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);
        Asset queriedAsset = await blockchain.GetAssetById(asset.GetId());

        Asset.Equals(asset, queriedAsset);
    }
}