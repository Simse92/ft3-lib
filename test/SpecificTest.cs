using Chromia.Postchain.Ft3;
using System.Collections.Generic;
using Xunit;

public class SpecificTest
{
    const string chainId = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
    const string nodeUrl = "http://localhost:7740";

    // should have one payment history entry when one transfer is made
    [Fact(Skip = "Working")]
    public async void SpecificTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        // VAULT Account -> A, T
        User vaultUser = TestUser.SingleSig();
        Account account = await Account.Register(vaultUser.AuthDescriptor, blockchain.NewSession(vaultUser));
        await AssetBalance.GiveBalance(account.Id, asset.GetId(), 150, blockchain);
        //System.Console.WriteLine("created:" + Util.ByteArrayToString(account.Id));

        // GAME AuthDescriptor -> T
        // GAME KeyPair!
        KeyPair keyPair = new KeyPair();
        SingleSignatureAuthDescriptor gameSingleSigAuthDescriptor = new SingleSignatureAuthDescriptor(
            keyPair.PubKey,
            new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
        );

        // VAULT adds GameAuthdescriptor to VAULT FT3 Account
        await account.AddAuthDescriptor(gameSingleSigAuthDescriptor);

        // LOGIN with localstorage keypair
        User impUser = new User(keyPair, gameSingleSigAuthDescriptor);
        Account[] impAccount = await Account.GetByAuthDescriptorId(gameSingleSigAuthDescriptor.ID, blockchain.NewSession(impUser));
        Account myAccount = impAccount[0];
        System.Console.WriteLine("imported:" + myAccount.AuthDescriptor.Count);

        await myAccount.Transfer(myAccount.Id, asset.GetId(), 10);
    }

}