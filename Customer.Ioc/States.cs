namespace StateMachine.Ioc
{
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
