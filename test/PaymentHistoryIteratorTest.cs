using Chromia.Postchain.Ft3;
using System.Collections.Generic;
using Xunit;

public class PaymentHistoryIteratorTest
{
    const string chainId = "61A42DF2FDED147AFBF3B14DCD6F34F9F1747B60C6EF248F4ECCCF5427A73041";
    const string nodeUrl = "http://localhost:7740";

    // should have one payment history entry when one transfer is made
    [Fact(Skip = "Working")]
    public async void PaymentHistoryIteratorTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(1);
        Account account1 = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder2.Build();

        await account1.Transfer(account2.Id, asset.GetId(), 10);

        PaymentHistoryIterator paymentHistoryIterator = await account1.GetPaymentHistoryIterator(5);
        PaymentHistoryEntry[] paymentHistoryEntries = paymentHistoryIterator.Next();

        Assert.Equal(1, paymentHistoryIterator.GetPageCount());
        Assert.Equal(1, paymentHistoryEntries.Length);

        foreach (var paymentHistoryEntry in paymentHistoryEntries)
        {
            System.Console.WriteLine(paymentHistoryEntry.Timestamp);
        }
    }

    // should have two payment history entries if two transfers made
    [Fact(Skip = "Working")]
    public async void PaymentHistoryIteratorTestRun2()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(2);
        Account account1 = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder2.Build();

        await account1.Transfer(account2.Id, asset.GetId(), 10);
        await account1.Transfer(account2.Id, asset.GetId(), 11);

        PaymentHistoryIterator paymentHistoryIterator = await account1.GetPaymentHistoryIterator(5);
        PaymentHistoryEntry[] paymentHistoryEntries = paymentHistoryIterator.Next();

        Assert.Equal(1, paymentHistoryIterator.GetPageCount());
        Assert.Equal(2, paymentHistoryEntries.Length);

        foreach (var paymentHistoryEntry in paymentHistoryEntries)
        {
            System.Console.WriteLine(paymentHistoryEntry.Delta);
        }
    }

    // should have two payment history entries when sender and receiver are the same
    [Fact(Skip = "Working")]
    public async void PaymentHistoryIteratorTestRun3()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(1);
        Account account1 = await accountBuilder.Build();

        await account1.Transfer(account1.Id, asset.GetId(), 15);

        PaymentHistoryIterator paymentHistoryIterator = await account1.GetPaymentHistoryIterator(5);
        PaymentHistoryEntry[] paymentHistoryEntries = paymentHistoryIterator.Next();

        Assert.Equal(1, paymentHistoryIterator.GetPageCount());
        Assert.Equal(2, paymentHistoryEntries.Length);

        var entry1 = paymentHistoryEntries[0];
        var entry2 = paymentHistoryEntries[1];

        Assert.Equal(false, entry1.IsInput);
        Assert.Equal(Util.ByteArrayToString(blockchain.Id), entry1.Other[0][0]);
        Assert.Equal(Util.ByteArrayToString(account1.Id), entry1.Other[0][1]);

        Assert.Equal(true, entry2.IsInput);
        Assert.Equal(Util.ByteArrayToString(blockchain.Id), entry2.Other[0][0]);
        Assert.Equal(Util.ByteArrayToString(account1.Id), entry2.Other[0][1]);
    }

    // should have more than one page if number of entries is greater than page size
    [Fact(Skip = "Working")]
    public async void PaymentHistoryIteratorTestRun4()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(4);
        Account account1 = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder2.Build();

        await account1.Transfer(account2.Id, asset.GetId(), 10);
        await account1.Transfer(account2.Id, asset.GetId(), 10);
        await account1.Transfer(account2.Id, asset.GetId(), 10);
        await account1.Transfer(account2.Id, asset.GetId(), 10);
        await account1.Transfer(account2.Id, asset.GetId(), 10);
        await account1.Transfer(account2.Id, asset.GetId(), 10);

        PaymentHistoryIterator paymentHistoryIterator = await account1.GetPaymentHistoryIterator(2);

        Assert.Equal(3, paymentHistoryIterator.GetPageCount());
    }   


    // should have one payment history entries if one crosschain transfer is made
    [Fact(Skip = "Working")]
    public async void PaymentHistoryIteratorTestRun5()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(1);
        Account account1 = await accountBuilder.Build();

        var chainId2 = TestUtil.GenerateId();
        var accountId2 = TestUtil.GenerateId();
        await account1.XcTransfer(chainId2, accountId2, asset.GetId(), 10);

        PaymentHistoryIterator paymentHistoryIterator = await account1.GetPaymentHistoryIterator(5);
        PaymentHistoryEntry[] paymentHistoryEntries = paymentHistoryIterator.Next();

        Assert.Equal(1, paymentHistoryIterator.GetPageCount());
        Assert.Equal(1, paymentHistoryEntries.Length);

        var entry1 = paymentHistoryEntries[0];
        Assert.Equal(Util.ByteArrayToString(chainId2), entry1.Other[0][0]);
        Assert.Equal(Util.ByteArrayToString(accountId2), entry1.Other[0][1]);
    }   

    //should have two payment history entries if one crosschain transfer and one transfer is made
    [Fact(Skip = "Working")]
    public async void PaymentHistoryIteratorTestRun6()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(2);
        Account account1 = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder2.Build();

        await account1.Transfer(account2.Id, asset.GetId(), 10);  
        await account1.XcTransfer(TestUtil.GenerateId(), TestUtil.GenerateId(), asset.GetId(), 11);

        PaymentHistoryIterator paymentHistoryIterator = await account1.GetPaymentHistoryIterator(5);
        PaymentHistoryEntry[] paymentHistoryEntries = paymentHistoryIterator.Next();

        Assert.Equal(1, paymentHistoryIterator.GetPageCount());
        Assert.Equal(2, paymentHistoryEntries.Length);

        foreach (var paymentHistoryEntry in paymentHistoryEntries)
        {
            System.Console.WriteLine(paymentHistoryEntry.BlockHeight);
        }
    }   
}