namespace ProductManagementSystem.Models
{
    public class PlaceOrderResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int OrderId { get; set; }
        public int RemainingStock { get; set; }
        public string? Status { get; set; }
    }
}
