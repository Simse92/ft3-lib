using System;
using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using Chromia.Postchain.Client;
using Xunit;


public class UserTest
{
    const string chainId = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
    const string nodeUrl = "http://localhost:7740";

    [Fact]
    public async void AccountTestRun()
    {
       Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);

       User user = TestUser.SingleSig();
       // Dev Account
       // Account account = await Account.Register(user.AuthDescriptor, blockchain.NewSession(user));

       // Dapp user
       await blockchain.Call(
            user,
            "create_user",
            "john_hoe",
            "John",
            "Doe",
            30,
            user.AuthDescriptor.ToGTV()
        );
    }
}