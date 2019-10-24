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
            var rawSource = rawTransfer["args"][0];
            var source = new TransferParam((string) rawSource[0], (string) rawSource[1], (float) rawSource[3]);
            var target = new XTransferTarget((string) rawTransfer["args"][1][0]);
            var hops = rawTransfer["args"][2];

            return new XTransferOperation(source, target, hops);
        }
    }
}