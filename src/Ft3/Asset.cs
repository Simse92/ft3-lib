using Chromia.Postchain.Client.GTV;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class Asset
    {
        public string Name;
        public byte[] ChainId;

        public Asset(string name, byte[] chainId)
        {
            this.Name = name;
            this.ChainId = chainId;
        }

        public byte[] GetId()
        {
            var body = new List<dynamic>(){this.Name, this.ChainId};
            return Gtv.Hash(body.ToArray());
        }

        public static async Task<Asset> Register(string name, byte[] chainId, Blockchain blockchain)
        {
            var tx = blockchain.Connection.Gtx.NewTransaction(new byte[][] {});
            tx.AddOperation("ft3.dev_register_asset", name, Util.ByteArrayToString(chainId), Util.ByteArrayToString(chainId));
            await tx.PostAndWaitConfirmation();
            return new Asset(name, chainId);
        }

        public static async Task<List<Asset>> GetByName(string name, Blockchain blockchain)
        {
            var assets = await blockchain.Connection.Gtx.Query("ft3.get_asset_by_name", ("name", name));
            List<Asset> assetList = new List<Asset>();

            foreach (var asset in assets)
            {
                assetList.Add(new Asset(asset["name"], Util.HexStringToBuffer(asset["issuing_chain_rid"])));
            }

            return assetList;
        }
    }

}