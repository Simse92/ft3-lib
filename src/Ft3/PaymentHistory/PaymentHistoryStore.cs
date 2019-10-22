namespace Chromia.Postchain.Ft3
{
    public interface PaymentHistoryStore
    {
        int GetCount(byte[] accountId);
        PaymentHistoryIterator GetIterator(byte[] accountId, int pageSize);
        void Save(byte[] accountId, PaymentHistoryEntry[] paymentHistoryEntries);
        PaymentHistoryEntry[] GetEntries(byte[] accountId, int start, int pageSize);
        dynamic GetSyncInfo(byte[] accountId);
        void SaveSyncInfo(byte[] accountId, dynamic syncInfo);
    }

}