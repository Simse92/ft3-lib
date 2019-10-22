namespace Chromia.Postchain.Ft3
{
    public class XTransferOperation
    {
        public readonly TransferParam Source;
        public readonly XTransferTarget Target;
        public readonly string[] Hops;

        public XTransferOperation(TransferParam source, XTransferTarget target, string[] hops)
        {
            this.Source = source;
            this.Target = target;
            this.Hops = hops;
        }

        public static XTransferOperation From(dynamic rawTransfer)
        {
            // ToDo
            return null;
        }
    }
}