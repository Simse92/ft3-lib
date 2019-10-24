using System.Threading.Tasks;
using Chromia.Postchain.Ft3;

public class BlockchainUtil
{
    public static async Task<Blockchain> GetDefaultBlockchain(string chainId, string nodeUrl)
    {
        return await Blockchain.Initialize(
            Util.HexStringToBuffer(chainId),
            DirectoryServiceUtil.GetDefaultDirectoryService(chainId, nodeUrl)
        );
    }
}