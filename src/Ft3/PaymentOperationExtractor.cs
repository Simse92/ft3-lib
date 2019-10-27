using Chromia.Postchain.Client.GTX;
using Chromia.Postchain.Client.GTX.ASN1Messages;

namespace Chromia.Postchain.Ft3
{
    public class PaymentOperationExtractor
    {
        private readonly byte[] _transaction;
        private readonly string _chainId;

        public PaymentOperationExtractor(byte[] rawTransaction, string chainId)
        {
            this._transaction = rawTransaction;
            this._chainId = chainId;
        }

        public PaymentOperation[] Extract()
        {
            System.Console.WriteLine("PaymentOperationExtractor: " + Util.ByteArrayToString(this._transaction));

            GTXValue value = Gtx.Deserialize(this._transaction);


            // System.Console.WriteLine("Val1");
            // System.Console.WriteLine(value.Array[0]);
            // System.Console.WriteLine("Val2");
            // System.Console.WriteLine(value.Array[1]);
            
            // Todo Not Supported
            // var transaction = gtx.deserialize(this.transaction);
            // GTXValue value = null;

            

            return null;
        }
    }
}