using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class BlockchainInfo
    {
        public string Name;
        public string Website;
        public string Descriptor;

        public BlockchainInfo(string name, string website, string descriptor)
        {
            this.Name = name;
            this.Website = website;
            this.Descriptor = descriptor;
        }

        public static async Task<BlockchainInfo> GetInfo(ConnectionClient connection)
        {
            System.Console.WriteLine("HIER123");
            var info = await connection.Query("ft3.get_blockchain_info");
            System.Console.WriteLine("HIER124");
            return new BlockchainInfo(info["name"], info["website"], info["description"]);
        }
    }

}