namespace StateMachine.Ioc
{
    public enum States
    {
        OrderPending,
        CustomerPending,
        CustomerApproved,
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
