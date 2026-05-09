namespace MizanERP.Domain.Enums
{
    public enum ProductType
    {
        RawMaterial = 1,
        FinishedGood = 2
    }

    public enum OrderStatus
    {
        Draft = 1,
        Submitted = 2,
        Approved = 3,
        Received = 4,
        Completed = 5,
        Cancelled = 6
    }

    public enum MovementType
    {
        In = 1,
        Out = 2,
        Transfer = 3,
        ProductionConsumption = 4,
        ProductionOutput = 5
    }

    public enum AccountType
    {
        Asset = 1,
        Liability = 2,
        Equity = 3,
        Revenue = 4,
        Expense = 5
    }
}