using System;
using System.Collections.Generic;
using Chromia.Postchain.Ft3;
using Xunit;


public class UserTest
{
    [Fact]
    public void UserTestRun()
    {
        Runner();

        while(true);
    }

    public async void Runner()
    {
        Console.WriteLine("HIER");
        Blockchain blockchain = await Blockchain.Initialize(
            Util.HexStringToBuffer("0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"),
            new DirectoryService()
        );

        Console.WriteLine("HIER1");
        var keyPair = new KeyPair();
        User user = new User(
            keyPair,
            new SingleSignatureAuthDescriptor(
                keyPair.PubKey,
                new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
            )
        );

        Console.WriteLine("HIER2");
        try
        {
            await blockchain.Call(
                user,
                "create_user",
                "John_Doe",
                "PETER",
                "MAFFAY",
                30,
                user.AuthDescriptor.ToGTV()
            );
        }
        catch (Exception)
        {
            Console.WriteLine("IPS");
        }
        Console.WriteLine("HIER3");

        var userAccount = await blockchain.Query("find_by_username", ("username", "John_Doe"));

        Console.WriteLine(userAccount);
    }
}

class DirectoryService : DirectoryServiceBase
{
    public DirectoryService() : base(
        (new List<ChainConnectionInfo>(){new ChainConnectionInfo(
            Util.HexStringToBuffer("0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"),
            "http://localhost:7740"
        )}).ToArray()
    ){}
}