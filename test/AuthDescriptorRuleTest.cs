using Chromia.Postchain.Ft3;
using System.Threading.Tasks;
using System;
using Xunit;

public class AuthDescriptorRuleTest
{
    const string chainId = "61A42DF2FDED147AFBF3B14DCD6F34F9F1747B60C6EF248F4ECCCF5427A73041";
    const string nodeUrl = "http://localhost:7740";

    public async Task<Account> SourceAccount(Blockchain blockchain, User user, Asset asset)
    {
        AccountBuilder builder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        builder.WithBalance(asset, 200);
        builder.WithPoints(5);
        return await builder.Build();
    }

    public async Task<Account> DestinationAccount(Blockchain blockchain)
    {
        AccountBuilder builder = AccountBuilder.CreateAccountBuilder(blockchain);
        return await builder.Build();
    }

    public async Task<Asset> CreateAsset(Blockchain blockchain)
    {
        return await Asset.Register(
            TestUtil.GenerateAssetName(),
            TestUtil.GenerateId(),
            blockchain
        );
    }

    // should succeed when calling operations, number of times less than or equal to value set by operation count rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.OperationCount().LessOrEqual(2));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);

        var opControl2 = await account1.Transfer(account2.GetID(), asset.GetId(), 20);
        Assert.Equal(false, opControl2.Error);
    }

    // should fail when calling operations, number of times more than value set by operation count rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun2()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.OperationCount().LessThan(2));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);

        var opControl2 = await account1.Transfer(account2.GetID(), asset.GetId(), 20);
        Assert.Equal(true, opControl2.Error);
    }

    // should fail when current time is greater than time defined by 'less than' block time rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun3()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockTime().LessThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 10000 ));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(true, opControl.Error);
    }

    // should succeed when current time is less than time defined by 'less than' block time rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun4()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);
        User user = TestUser.SingleSig(Rules.BlockTime().LessThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 10000 ));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);
    }

    // should succeed when current block height is less than value defined by 'less than' block height rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun5()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockHeight().LessThan(10000));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);
    }

    // should fail when current block height is greater than value defined by 'less than' block height rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun6()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockHeight().LessThan(1));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(true, opControl.Error);
    }

    // should fail if operation is executed before timestamp defined by 'greater than' block time rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun7()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockTime().GreaterThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 10000 ));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(true, opControl.Error);
    }

    // should succeed if operation is executed after timestamp defined by 'greater than' block time rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun8()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockTime().GreaterThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 10000 ));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);
    }

    // should fail if operation is executed before block defined by 'greater than' block height rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun9()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockHeight().GreaterThan(10000));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(true, opControl.Error);
    }

    // should succeed if operation is executed after block defined by 'greater than' block height rule
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun10()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockHeight().GreaterThan(1));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);
    }

    // should be able to create complex rules
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun11()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockHeight().GreaterThan(1).And().BlockHeight().LessThan(10000));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);
    }

    // should fail if block heights defined by 'greater than' and 'less than' block height rules are less than current block height
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun12()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(Rules.BlockHeight().GreaterThan(1).And().BlockHeight().LessThan(10));
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(true, opControl.Error);
    }

    // should fail if block times defined by 'greater than' and 'less than' block time rules are in the past
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun13()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(
            Rules.BlockTime().GreaterThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 20000).
            And().
            BlockTime().LessThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 10000)
            );
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(true, opControl.Error);
    }

    // should succeed if current time is within period defined by 'greater than' and 'less than' block time rules
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun14()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user = TestUser.SingleSig(
            Rules.BlockTime().GreaterThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 10000).
            And().
            BlockTime().LessThan(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 10000)
            );
        Account account1 = await SourceAccount(blockchain, user, asset);
        Account account2 = await DestinationAccount(blockchain);

        var opControl = await account1.Transfer(account2.GetID(), asset.GetId(), 10);
        Assert.Equal(false, opControl.Error);
    }

    // should succeed if current time is within period defined by 'greater than' and 'less than' block time rules
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun15()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig(Rules.OperationCount().LessThan(2));

        Account srcAccount1 = await SourceAccount(blockchain, user1, asset);
        Account destAccount = await DestinationAccount(blockchain);

        // add expiring auth descriptor to the account
        await srcAccount1.AddAuthDescriptor(user2.AuthDescriptor);

        // get the same account, but initialized with user2
        // object which contains expiring auth descriptor
        var srcAccount2 = await blockchain.NewSession(user2).GetAccountById(srcAccount1.GetID());

        await srcAccount2.Transfer(destAccount.GetID(), asset.GetId(), 10);

        // account descriptor used by user2 object has expired.
        // this operation call will delete it.
        // any other operation, which calls require_auth internally
        // would also delete expired auth descriptor.
        await srcAccount1.Transfer(destAccount.GetID(), asset.GetId(), 30);

        await srcAccount1.Sync();

        Assert.Equal(1, srcAccount1.AuthDescriptor.Count);
    }

    // shouldn't delete non-expired auth descriptor
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun16()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig(Rules.OperationCount().LessThan(10));

        Account srcAccount1 = await SourceAccount(blockchain, user1, asset);
        Account destAccount = await DestinationAccount(blockchain);

        // add expiring auth descriptor to the account
        await srcAccount1.AddAuthDescriptor(user2.AuthDescriptor);

        // get the same account, but initialized with user2
        // object which contains expiring auth descriptor
        var srcAccount2 = await blockchain.NewSession(user2).GetAccountById(srcAccount1.GetID());
        
        // perform transfer with expiring auth descriptor.
        // auth descriptor didn't expire, because it's only used 1 out of 10 times.
        await srcAccount2.Transfer(destAccount.GetID(), asset.GetId(), 10);

        // perform transfer using auth descriptor without rules
        await srcAccount1.Transfer(destAccount.GetID(), asset.GetId(), 10);

        await srcAccount1.Sync();

        Assert.Equal(2, srcAccount1.AuthDescriptor.Count);
    }

    // should delete only expired auth descriptor if multiple expiring descriptors exist
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun17()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig(Rules.OperationCount().LessOrEqual(1));
        User user3 = TestUser.SingleSig(Rules.OperationCount().LessOrEqual(1));

        Account srcAccount1 = await SourceAccount(blockchain, user1, asset);
        Account destAccount = await DestinationAccount(blockchain);

        await srcAccount1.AddAuthDescriptor(user2.AuthDescriptor);
        await srcAccount1.AddAuthDescriptor(user3.AuthDescriptor);

        var srcAccount2 = await blockchain.NewSession(user2).GetAccountById(srcAccount1.GetID());
        
        await srcAccount2.Transfer(destAccount.GetID(), asset.GetId(), 50);

        // this call will trigger deletion of expired auth descriptor (attached to user2)
        await srcAccount1.Transfer(destAccount.GetID(), asset.GetId(), 100);

        await srcAccount1.Sync();

        Assert.Equal(2, srcAccount1.AuthDescriptor.Count);
    }

    // should add auth descriptors
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun18()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig(Rules.OperationCount().LessOrEqual(1));
        User user3 = TestUser.SingleSig(Rules.OperationCount().LessOrEqual(1));

        Account account = await SourceAccount(blockchain, user1, asset);

        await account.AddAuthDescriptor(user2.AuthDescriptor);
        await account.AddAuthDescriptor(user3.AuthDescriptor);

        await account.Sync();

        Assert.Equal(3, account.AuthDescriptor.Count);
    }

    // should delete auth descriptors
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun19()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig(Rules.OperationCount().LessOrEqual(1));
        User user3 = TestUser.SingleSig(Rules.OperationCount().LessOrEqual(1));

        Account account = await SourceAccount(blockchain, user1, asset);

        await account.AddAuthDescriptor(user2.AuthDescriptor);
        await account.AddAuthDescriptor(user3.AuthDescriptor);

        await account.DeleteAllAuthDescriptorsExclude(user1.AuthDescriptor);
        Assert.Equal(1, account.AuthDescriptor.Count);

        await account.Sync();

        Assert.Equal(1, account.AuthDescriptor.Count);
    }

    // should fail when deleting an auth descriptor which is not owned by the account
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun20()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig();

        Account account1 = await SourceAccount(blockchain, user1, asset);
        await SourceAccount(blockchain, user2, asset);
        
        var opControl = await account1.DeleteAuthDescriptor(user2.AuthDescriptor);

        Assert.Equal(true, opControl.Error);
    }

    // should delete auth descriptor
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun21()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig();

        Account account = await SourceAccount(blockchain, user1, asset);
        await account.AddAuthDescriptor(user2.AuthDescriptor);
        await account.DeleteAuthDescriptor(user2.AuthDescriptor);

        Assert.Equal(1, account.AuthDescriptor.Count);
    }

    // Should be able to create same rules with different value
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun22()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        var rules = Rules.BlockHeight().GreaterThan(1).And().BlockHeight().GreaterThan(10000).And().BlockTime().GreaterOrEqual(122222999);
        User user = TestUser.SingleSig(rules);

        Account account = await SourceAccount(blockchain, user, asset);

        Assert.Equal(1, account.AuthDescriptor.Count);
    }

    // shouldn't be able to create too many rules
    [Fact(Skip = "Working")]
    public async void AuthDescriptorRuleTestRun23()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await CreateAsset(blockchain);

        var rules = Rules.BlockHeight().GreaterThan(1).And().BlockHeight().GreaterThan(10000).And().BlockTime().GreaterOrEqual(122222999);
       
        for (int i = 0; i < 400; i++)
        {
            rules = rules.And().BlockHeight().GreaterOrEqual(i);
        }
       
        User user = TestUser.SingleSig(rules);

        Account account = await SourceAccount(blockchain, user, asset);

        Assert.Equal(null, account);
    }
}