using System.Collections.Generic;
using System;

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
                if(arg is AuthDescriptor)
                {
                    System.Console.WriteLine("AuthDescriptorarg: " + arg);
                    AuthDescriptor authDescriptor = (AuthDescriptor) arg;
                    gtvList.Add(authDescriptor.ToGTV());
                }
                else if(arg is System.Byte[])
                {
                    System.Console.WriteLine("System.Byte[]: " + arg);
                    gtvList.Add(Util.ByteArrayToString(arg));
                }
                else if(arg is byte[])
                {
                    System.Console.WriteLine("byte[]: " + arg);
                    gtvList.Add(Util.ByteArrayToString(arg));
                }
                else if(arg is string)
                {
                    System.Console.WriteLine("string: " + arg);
                    gtvList.Add(arg);
                }
                else if(arg is int)
                {
                    System.Console.WriteLine("int: " + arg);
                    gtvList.Add(arg);
                }
                else if(arg is System.Array)
                {
                    System.Console.WriteLine("System.Array: " + arg);

                    // gtvList.AddRange(ToGTV(arg, ++layer));
                    gtvList.Add(arg);
                }
                else
                {
                    Console.WriteLine("Wrong Datatype.");
                }
            }

            return gtvList;
        }

        public Transaction Build(List<byte[]> signers)
        {
            var tx = this.Blockchain.Connection.Gtx.NewTransaction(signers.ToArray());
            foreach (var operation in this._operations)
            {
                var type = (string) operation[0];
                operation.Remove(operation[0]);

                tx.AddOperation(type, operation.ToArray());
            }
            return new Transaction(tx, this.Blockchain);
        }
    }
}