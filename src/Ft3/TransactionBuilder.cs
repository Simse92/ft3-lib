using System.Collections.Generic;

namespace Chromia.Postchain.Ft3
{
    public class TransactionBuilder
    {
        private List<dynamic> _operations;
        public readonly Blockchain Blockchain;
        public TransactionBuilder(Blockchain blockchain)
        {
            this.Blockchain = blockchain;
            _operations = new List<dynamic>();
        }

        public TransactionBuilder AddOperation(params dynamic[] args)
        {
            this._operations.Add(ToGTV(args));
            return this;
        }

        private List<dynamic> ToGTV(dynamic[] args)
        {
            List<dynamic> gtvList = new List<dynamic>();

            foreach (var arg in args)
            {
                if(arg is System.Array)
                {
                    gtvList.AddRange(ToGTV(arg));
                }
                else if(arg is byte[])
                {
                    gtvList.Add(Util.ByteArrayToString(arg));
                }
                else if(arg is string)
                {
                    gtvList.Add(arg);
                }
                else if(arg is int)
                {
                    gtvList.Add(arg);
                }
            }

            return gtvList;
        }

        public Transaction Build(List<byte[]> signers)
        {
            var tx = this.Blockchain.Connection.Gtx.NewTransaction(signers.ToArray());
            foreach (var operation in this._operations)
            {
                tx.AddOperation(operation);
            }
            return new Transaction(tx, this.Blockchain);
        }
    }
}