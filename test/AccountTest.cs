using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using Xunit;


public class UserTest
{
    const string chainId = "6539EC234FC62BE2B3F6C8B391FC4BBAA75455DAEF1F32CD0D3674BADEE8F19F";
    const string nodeUrl = "http://localhost:7740";

    // Correctly creates keypair
    [Fact(Skip = "Working")]
    public void AccountTest1()
    {
        var keyPair = Chromia.Postchain.Client.Util.MakeKeyPair();
        var user = new KeyPair(Util.ByteArrayToString(keyPair["privKey"]));

        Assert.Equal(user.PrivKey, keyPair["privKey"]);  
        Assert.Equal(user.PubKey, keyPair["pubKey"]);  
    }

    // Register account on blockchain
    [Fact(Skip = "Working")]
    public async void AccountTest2()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();
        Account account = await Account.Register(user.AuthDescriptor, blockchain.NewSession(user));

        Assert.NotNull(account);
    }

    // can add new auth descriptor if has account edit rights
    [Fact(Skip = "Working")]
    public async void AccountTest3()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithPoints(1);
        Account account = await accountBuilder.Build();

        Assert.NotNull(account);

        await account.AddAuthDescriptor( new SingleSignatureAuthDescriptor(
                user.KeyPair.PubKey,
                new List<FlagsType>(){FlagsType.Transfer}.ToArray()
        ));
        Assert.Equal(2, account.AuthDescriptor.Count);
    }

    // cannot add new auth descriptor if account doesn't have account edit rights
    [Fact(Skip = "Working")]
    public async void AccountTest4()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();
        Account account = await Account.Register(
            new SingleSignatureAuthDescriptor(
                user.KeyPair.PubKey,
                new List<FlagsType>(){FlagsType.Transfer}.ToArray()
            ),
            blockchain.NewSession(user)
        );
        Assert.NotNull(account);

        var response = await account.AddAuthDescriptor(
            new SingleSignatureAuthDescriptor(
                user.KeyPair.PubKey,
                new List<FlagsType>(){FlagsType.Transfer}.ToArray()
            )
        );
        Assert.Equal(true, response.Error);
        Assert.Equal(1, account.AuthDescriptor.Count);
    }

    // should create new multisig account
    [Fact(Skip = "Working")]
    public async void AccountTest5()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig();

        Account account = await Account.Register(
            new MultiSignatureAuthDescriptor(
                new List<byte[]>(){
                    user1.KeyPair.PubKey, user2.KeyPair.PubKey
                },
                2,
                new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
            ),
            blockchain.NewSession(user1)
        );

        Assert.NotNull(account);
    }

    // should update account if 2 signatures provided
    [Fact(Skip = "Working")]
    public async void AccountTest6()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig();

        Account account = await Account.Register(
            new MultiSignatureAuthDescriptor(
                new List<byte[]>(){
                    user1.KeyPair.PubKey, user2.KeyPair.PubKey
                },
                2,
                new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
            ),
            blockchain.NewSession(user1)
        );

        Assert.NotNull(account);
        AuthDescriptor authDescriptor = new SingleSignatureAuthDescriptor(
                user1.KeyPair.PubKey,
                new List<FlagsType>(){FlagsType.Transfer}.ToArray()
        );
        await account.AddAuthDescriptor(authDescriptor);

        Assert.Equal(2, account.AuthDescriptor.Count);
    }

    // should fail if only one signature provided
    [Fact(Skip = "Working")]
    public async void AccountTest7()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig();

        Account account = await Account.Register(
            new MultiSignatureAuthDescriptor(
                new List<byte[]>(){
                    user1.KeyPair.PubKey, user2.KeyPair.PubKey
                },
                2,
                new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
            ),
            blockchain.NewSession(user1)
        );

        Assert.NotNull(account);

        await account.AddAuthDescriptor(
            new SingleSignatureAuthDescriptor(
                user1.KeyPair.PubKey,
                new List<FlagsType>(){FlagsType.Transfer}.ToArray()
            )
        );

        Assert.Equal(1, account.AuthDescriptor.Count);
    }

    // should be returned when queried by participant id
    [Fact(Skip = "Working")]
    public async void AccountTest8()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();
        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        Account account = await accountBuilder.Build();
        
        Account[] accounts = await Account.GetByParticipantId(user.KeyPair.PubKey, blockchain.NewSession(user));
        Assert.Equal(1, accounts.Length);
        Assert.Equal(Util.ByteArrayToString(user.KeyPair.PubKey), Util.ByteArrayToString(accounts[0].AuthDescriptor[0].PubKey[0]));
    }

    // should return two accounts when account is participant of two accounts
    [Fact(Skip = "Working")]
    public async void AccountTest9()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain);
        accountBuilder.WithParticipants(new List<KeyPair>(){user1.KeyPair});
        Account account = await accountBuilder.Build();

        AccountBuilder accountBuilder2 = AccountBuilder.CreateAccountBuilder(blockchain, user2);
        accountBuilder2.WithParticipants(new List<KeyPair>(){user2.KeyPair});
        accountBuilder2.WithPoints(1);
        Account account2 = await accountBuilder2.Build();

        await account2.AddAuthDescriptor(
            new SingleSignatureAuthDescriptor(user1.KeyPair.PubKey, new List<FlagsType>(){FlagsType.Transfer}.ToArray())
        );

        Account[] accounts = await Account.GetByParticipantId(user1.KeyPair.PubKey, blockchain.NewSession(user1));

        Assert.Equal(2, accounts.Length);
    }

    // should return account by id
    [Fact(Skip = "Working")]
    public async void AccountTest10()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        Account account = await accountBuilder.Build();

        var foundAccount = await Account.GetById(account.Id, blockchain.NewSession(user));
        Assert.Equal(account.Id, foundAccount.Id);
    }

    // should have only one auth descriptor after calling deleteAllAuthDescriptorsExclude
    [Fact(Skip = "Working")]
    public async void AccountTest11()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        User user1 = TestUser.SingleSig();
        User user2 = TestUser.SingleSig();
        User user3 = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user1);
        accountBuilder.WithParticipants(new List<KeyPair>(){user1.KeyPair});
        accountBuilder.WithPoints(3);
        Account account = await accountBuilder.Build();

        AuthDescriptor authDescriptor1 = new SingleSignatureAuthDescriptor(
            user2.KeyPair.PubKey,
            new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
        );

        AuthDescriptor authDescriptor2 = new SingleSignatureAuthDescriptor(
            user3.KeyPair.PubKey,
            new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
        );

        await account.AddAuthDescriptor(authDescriptor1);
        await account.AddAuthDescriptor(authDescriptor2);

        await account.DeleteAllAuthDescriptorsExclude(user1.AuthDescriptor);

        Account foundAccount = await blockchain.NewSession(user1).GetAccountById(account.Id);
        Assert.Equal(1, foundAccount.AuthDescriptor.Count);
    }

    // should be able to register account by directly calling \'register_account\' operation
    [Fact(Skip = "Working")]
    public async void AccountTest12()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

        User user = TestUser.SingleSig();

        await blockchain.Call(AccountOperations.Op("ft3.dev_register_account", new List<dynamic>() {
            user.AuthDescriptor
        }.ToArray()
        )
        , user);
    
        BlockchainSession session = blockchain.NewSession(user);
        Account account = await session.GetAccountById(user.AuthDescriptor.ID);

        Assert.NotNull(account);
    }
}