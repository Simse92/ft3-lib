using Chromia.Postchain.Client;
using Chromia.Postchain.Client.GTX;
using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class ConnectionClient
    {
        public string ChainUrl;
        public string ChainID;
        public GTXClient Gtx;

        public ConnectionClient(string chainURL, string chainID)
        {
            this.ChainUrl = chainURL;
            this.ChainID = chainID;

            var restClient = new RESTClient(chainURL, chainID);
            this.Gtx = new GTXClient(restClient, chainID);
        }

        public async Task<dynamic> Query(string name, params dynamic[] queryObject)
        {
            return await this.Gtx.Query(name, queryObject);
        }
    }
}