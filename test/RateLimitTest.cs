using Chromia.Postchain.Ft3;
using System.Threading.Tasks;
using System.Collections.Generic;
using Chromia.Postchain.Client;
using System;
using Xunit;

public class RateLimitTest
{
    const string chainId = "61A42DF2FDED147AFBF3B14DCD6F34F9F1747B60C6EF248F4ECCCF5427A73041";
    const string nodeUrl = "http://localhost:7740";

    const int REQUEST_MAX_COUNT = 0;
    const int RECOVERY_TIME = 0;


    // Should have a limit of 10 requests per minute
    [Fact(Skip = "Working")]
    public async void RateLimitTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        var info = await BlockchainInfo.GetInfo(blockchain.Connection);

        Assert.Equal(REQUEST_MAX_COUNT, info.RequestMaxCount);
        Assert.Equal(RECOVERY_TIME, info.RequestRecoveryTime);
    }

    // should show 10 at request count
    [Fact(Skip = "Working")]
    public async void RateLimitTestRun2()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder builder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        builder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        Account account = await builder.Build();

        await account.Sync();
        Assert.Equal(0, account.RateLimit.Points);
    }

    // waits 20 seconds and gets 4 points
    [Fact(Skip = "Working")]
    public async void RateLimitTestRun3()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder builder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        builder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        Account account = await builder.Build();

        await Task.Delay(TimeSpan.FromSeconds(20));

        await RateLimit.ExecFreeOperation(account.GetID(), blockchain); // used to make one block
        await RateLimit.ExecFreeOperation(account.GetID(), blockchain); // used to calculate the last block's timestamp (previous block).
        // check the balance
        await account.Sync();
        Assert.Equal(8, account.RateLimit.Points); // 20 seconds / 5s recovery time
    }

    // can make 4 operations
    [Fact(Skip = "Not Working")]
    public async void RateLimitTestRun4()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder builder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        builder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        builder.WithPoints(2);
        Account account = await builder.Build();

        var opControl = await MakeRequests(blockchain, account, 4);
        Assert.Equal(false, opControl.Error);

        await account.Sync();
        Assert.Equal(0, account.RateLimit.Points);
    }

    // can't make another operation because she has 0 points
    [Fact(Skip = "Working")]
    public async void RateLimitTestRun5()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder builder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        builder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        builder.WithPoints(2);
        Account account = await builder.Build();

        var opControl = await MakeRequests(blockchain, account, 4);
        Assert.Equal(false, opControl.Error);
        
        await account.Sync();
        
        var opControl2 = await MakeRequests(blockchain, account, 8);
        Assert.Equal(true, opControl2.Error);
    }


    public async Task<PostchainErrorControl> MakeRequests(Blockchain blockchain, Account account, int requests)
    {
        TransactionBuilder txBuilder = blockchain.CreateTransactionBuilder();
        for (int i = 0; i < requests; i++)
        {
            var disposableKeypair = TestUser.SingleSig();
            txBuilder.AddOperation(AccountOperations.AddAuthDescriptor(account.Session.User.AuthDescriptor.ID, account.Session.User.AuthDescriptor.ID, disposableKeypair.AuthDescriptor));
        }

        var tx = txBuilder.Build(account.Session.User.AuthDescriptor.Signers);
        tx.Sign(account.Session.User.KeyPair);
        return await tx.Post();
    }
}