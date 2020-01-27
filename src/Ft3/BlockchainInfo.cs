using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class BlockchainInfo
    {
        public string Name;
        public string Website;
        public string Descriptor;
        public int RequestMaxCount;
        public int RequestRecoveryTime;

        public BlockchainInfo(string name, string website, string descriptor, int requestMaxCount, int requestRecoveryTime)
        {
            this.Name = name;
            this.Website = website;
            this.Descriptor = descriptor;
            this.RequestMaxCount = requestMaxCount;
            this.RequestRecoveryTime = requestRecoveryTime;
        }

        public static async Task<BlockchainInfo> GetInfo(ConnectionClient connection)
        {
            try
            {
                var info = await connection.Query("ft3.get_blockchain_info");
                return new BlockchainInfo((string) info["name"], (string) info["website"], (string) info["description"], (int) info["request_max_count"], (int) info["request_recovery_time"]);
            }
            catch
            {
                 return new BlockchainInfo(connection.ChainID, null, null, 0, 0);
            }
        }
    }

}