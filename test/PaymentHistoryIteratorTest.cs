using Chromia.Postchain.Ft3;
using System.Collections.Generic;
using Xunit;

public class PaymentHistoryIteratorTest
{
    const string chainId = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
    const string nodeUrl = "http://localhost:7740";

    // should have one payment history entry when one transfer is made
    [Fact]
    public async void PaymentHistoryIteratorTestRun1()
    {
        Blockchain blockchain = await BlockchainUtil.GetDefaultBlockchain(chainId, nodeUrl);
        Asset asset = await Asset.Register(TestUtil.GenerateAssetName(), TestUtil.GenerateId(), blockchain);

        User user = TestUser.SingleSig();

        AccountBuilder accountBuilder = AccountBuilder.CreateAccountBuilder(blockchain, user);
        accountBuilder.WithParticipants(new List<KeyPair>(){user.KeyPair});
        accountBuilder.WithBalance(asset, 200);
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
            System.Console.WriteLine(paymentHistoryEntry);
        }

    }

}