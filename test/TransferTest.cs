using System;
using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using Xunit;


public class TransferTest
{
    const string chainId = "61A42DF2FDED147AFBF3B14DCD6F34F9F1747B60C6EF248F4ECCCF5427A73041";
    const string nodeUrl = "http://localhost:7740";

    // should succeed when balance is higher than amount to transfer
    [Fact(Skip = "Working")]
    public async void TransferTestRun1()
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

        AssetBalance assetBalance1 = await AssetBalance.GetByAccountAndAssetId(account1.Id, asset.GetId(), blockchain); 
        AssetBalance assetBalance2 = await AssetBalance.GetByAccountAndAssetId(account2.Id, asset.GetId(), blockchain);

        Console.WriteLine("AssetBalance1: " + assetBalance1.Amount);
        Console.WriteLine("AssetBalance2: " + assetBalance2.Amount);

        Assert.Equal(190, assetBalance1.Amount);  
        Assert.Equal(10, assetBalance2.Amount);
    }
    
    // should fail when balance is lower than amount to transfer
    [Fact(Skip = "Working")]
    public async void TransferTestRun2()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 5);
        Account account1 = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder.Build();

        await account1.Transfer(account2.Id, asset.GetId(), 10);
    }

    // should fail if auth descriptor doesn't have transfer rights
    [Fact(Skip = "Working")]
    public async void TransferTestRun3()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithAuthFlags(new List<FlagsType>(){FlagsType.Account});
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(1);
        Account account1 = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder2.Build();

        await account1.Transfer(account2.Id, asset.GetId(), 10);
    }

    // should succeed if transferring tokens to a multisig account
    [Fact(Skip = "Working")]
    public async void TransferTestRun4()
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
        accountBuilder2.WithParticipants(new List<KeyPair>(){new KeyPair(), new KeyPair()});
        accountBuilder2.WithRequiredSignatures(2);
        Account account2 = await accountBuilder2.Build();

        await account1.Transfer(account2.Id, asset.GetId(), 10);

        AssetBalance assetBalance1 = await AssetBalance.GetByAccountAndAssetId(account1.Id, asset.GetId(), blockchain); 
        AssetBalance assetBalance2 = await AssetBalance.GetByAccountAndAssetId(account2.Id, asset.GetId(), blockchain);

        Console.WriteLine("AssetBalance1: " + assetBalance1.Amount);
        Console.WriteLine("AssetBalance2: " + assetBalance2.Amount);

        Assert.Equal(190, assetBalance1.Amount);  
        Assert.Equal(10, assetBalance2.Amount);
    }

    // should succeed burning tokens
    [Fact(Skip = "Working")]
    public async void TransferTestRun5()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(1);
        Account account = await accountBuilder.Build();

        await account.BurnTokens(asset.GetId(), 10);
        AssetBalance assetBalance = account.GetAssetById(asset.GetId());
        Console.WriteLine("AssetBalance: " + assetBalance.Amount);
        
        Assert.Equal(190, assetBalance.Amount);
    }

    // should have one payment history entry if one transfer made
    [Fact(Skip = "Working")]
    public async void TransferTestRun6()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(1);
        Account account = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder2.Build();

        await account.Transfer(account2.Id, asset.GetId(), 10);

        PaymentHistoryEntryShort[] paymentHistory = await account.GetPaymentHistory();
        Assert.Equal(1, paymentHistory.Length);
    }

    // should have two payment history entries if two transfers made
    [Fact(Skip = "Working")]
    public async void TransferTestRun7()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
        accountBuilder.WithPoints(2);
        Account account = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain);
        Account account2 = await accountBuilder2.Build();

        await account.Transfer(account2.Id, asset.GetId(), 10);
        await account.Transfer(account2.Id, asset.GetId(), 11);

        PaymentHistoryEntryShort[] paymentHistory = await account.GetPaymentHistory();
        Assert.Equal(2, paymentHistory.Length);
    }
}