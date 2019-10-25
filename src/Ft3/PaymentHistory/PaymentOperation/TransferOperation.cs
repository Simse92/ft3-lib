using System.Collections.Generic;
using System.Linq;

namespace Chromia.Postchain.Ft3
{
    public class TransferOperation
    {
        public readonly TransferParam[] Inputs;
        public readonly TransferParam[] Outputs;

        public TransferOperation(IEnumerable<TransferParam> inputs, IEnumerable<TransferParam> outputs)
        {
            this.Inputs = inputs.ToArray();
            this.Outputs = outputs.ToArray();
        }

        public static TransferOperation From(dynamic rawTransfer)
        {
            var inputs = new List<TransferParam>();
            var outputs = new List<TransferParam>();

            foreach (var input in rawTransfer["args"][0])
            {
                inputs.Add(
                    new TransferParam((string) input[0], (string) input[1], (int) input[3])
                );
            }

            foreach (var input in rawTransfer["args"][1])
            {
                outputs.Add(
                    new TransferParam((string) input[0], (string) input[1], (int) input[2])
                );
            }
        
           return new TransferOperation(inputs, outputs);
        }
    }
}