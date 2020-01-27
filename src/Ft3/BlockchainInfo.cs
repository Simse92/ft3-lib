using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class BlockchainInfo
    {
        public string Name;
        public string Website;
        public string Description;
        public int RequestMaxCount;
        public int RequestRecoveryTime;

        public BlockchainInfo(string name, string website, string description, int requestMaxCount, int requestRecoveryTime)
        {
            this.Name = name;
            this.Website = website;
            this.Description = description;
            this.RequestMaxCount = requestMaxCount;
            this.RequestRecoveryTime = requestRecoveryTime;
        }

        public static async Task<BlockchainInfo> GetInfo(ConnectionClient connection)
        {
            var info = await connection.Query<BlockchainInfo>("ft3.get_blockchain_info");

            if(info.control.Error)
            {
                return new BlockchainInfo(connection.ChainID, null, null, 0, 0);
            }

            return info.content;
        }
    }
}