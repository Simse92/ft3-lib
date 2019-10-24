using System.Threading.Tasks;
using System.Collections.Generic;

namespace Chromia.Postchain.Ft3
{
    public class AssetBalance
    {
        public float Amount;
        public Asset Asset;

        public AssetBalance(float amount, Asset asset)
        {
            this.Amount = amount;
            this.Asset = asset;
        }

        public static async Task<List<AssetBalance>> GetByAccountId(byte[] id, Blockchain blockchain)
        {
            var assets = await blockchain.Connection.Gtx.Query("ft3.get_asset_balances", ("account_id", Util.ByteArrayToString(id)));
            System.Console.WriteLine(assets);
            List<AssetBalance> assetsBalances = new List<AssetBalance>();

            foreach (var asset in assets)
            {
                assetsBalances.Add(
                    new AssetBalance(
                        (float) asset["amount"],
                        new Asset(
                            (string) asset["name"],
                            Util.HexStringToBuffer((string) asset["chain_id"])
                            )                    
                    )
                );
            }
            return assetsBalances;
        }
    
        public static async Task<AssetBalance> GetByAccountAndAssetId(byte[] accountId, byte[] assetId, Blockchain blockchain)
        {
            var asset = await blockchain.Connection.Gtx.Query("ft3.get_asset_balance", 
                                                                ("account_id", Util.ByteArrayToString(accountId)),
                                                                ("asset_id", Util.ByteArrayToString(assetId))
            );

            if(asset == null)
            {
                return null;
            }

            return new AssetBalance(asset["amount"], new Asset(asset["name"], asset["chainId"]));
        }

        public static async Task GiveBalance(byte[] accountId, byte[] assetId, float amount, Blockchain blockchain)
        {
            var tx = blockchain.Connection.Gtx.NewTransaction(new byte[][] {});
            tx.AddOperation("ft3.dev_give_balance", Util.ByteArrayToString(assetId), Util.ByteArrayToString(accountId), amount);
            await tx.PostAndWaitConfirmation();
        }
    }

}