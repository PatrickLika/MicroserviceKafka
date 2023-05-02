
namespace Customer.Application.Queries
{
    public class ReadCvrDto
    {
        public string Id { get; set; }
        public int Screws { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public int Price { get; set; }
        public string Cvr { get; set; }
        public States State { get; set; }

        public enum States
        {
            OrderPending,
            CustomerPending,
            CustomerApproved,
            CustomerDenied,
            StoragePending,
            StorageApproved,
            PaymentPending,
            PaymentApproved,
            ReceiptPending,
            ReceiptDone,
            OrderApproved,
            OrderSuccessful
        }
    }
}
