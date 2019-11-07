using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using Xunit;

public class BlockchainTest
{
    const string chainId = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
    const string nodeUrl = "http://localhost:7740";

    // should provide info
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        BlockchainInfo info = await BlockchainInfo.GetInfo(blockchain.Connection);

        Assert.Equal(info.Name, "ChromaToken");
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

        var parsed = new List<string>()  {
            Util.ByteArrayToString(linkedChains[0]),
            Util.ByteArrayToString(linkedChains[1])
        };
    }

    // should return false when isLinkedWithChain is called for unknown chain id
    [Fact(Skip = "Working")]
    public async void BlockchainTestRun7()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

        Assert.Equal(false, await blockchain.IsLinkedWithChain(TestUtil.GenerateId()));
    }
}