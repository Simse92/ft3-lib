using System.Threading.Tasks;
using System.Collections.Generic;

namespace Chromia.Postchain.Ft3
{
    public class PaymentHistory
    {
        public static async Task<PaymentHistoryEntryShort[]> GetAccountById(byte[] id, ConnectionClient connection, int afterBlock = -1)
        {
            var paymentHistoryEntriesQuery = await connection.Gtx.Query(
                "ft3.get_payment_history",
                ("account_id", Util.ByteArrayToString(id)),
                ("after_block", afterBlock)
            );
            
            System.Console.WriteLine("PaymentHistory: " + paymentHistoryEntriesQuery);
            
            var paymentHistoryEntriesList = new List<PaymentHistoryEntryShort>();

            foreach (var entry in paymentHistoryEntriesQuery)
            {
                paymentHistoryEntriesList.Add(PaymentHistoryEntryShort.From(entry));
            }
            return paymentHistoryEntriesList.ToArray();
        }
    }
}