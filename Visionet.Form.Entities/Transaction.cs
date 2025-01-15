namespace Visionet.Form.Entities
{
    public class Transaction
    {
        public string Id { get; set; } = string.Empty;
        public string TypeCustomer { get; set; } = string.Empty;
        public int TotalGrocery { get; set; }
        public int Point { get; set; }
        public int Discount { get; set; }
        public int TotalPayment { get; set; }

    }
}
