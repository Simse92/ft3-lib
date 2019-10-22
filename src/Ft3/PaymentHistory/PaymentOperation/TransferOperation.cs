namespace Chromia.Postchain.Ft3
{
    public class TransferOperation
    {
        public readonly TransferParam[] Inputs;
        public readonly TransferParam[] Outputs;

        public TransferOperation(TransferParam[] inputs, TransferParam[] outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
        }

        public static TransferOperation From(dynamic rawTransfer)
        {
            // ToDo
           return null;
        }
    }
}