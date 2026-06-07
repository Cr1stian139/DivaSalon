namespace DivaSalon.Models.ViewModels
{
    public class OrderViewModel
    {
        public List<Service> AvailableServices { get; set; } = new();
        public List<Barber> AvailableBarbers { get; set; } = new();
        public Order Order { get; set; } = new();
        public List<int> SelectedServiceIds { get; set; } = new();
    }

    public class AdminOrderViewModel
    {
        public List<Order> PendingOrders { get; set; } = new();
        public List<Order> ConfirmedOrders { get; set; } = new();
        public List<Order> InProgressOrders { get; set; } = new();
        public List<Order> CompletedOrders { get; set; } = new();
        public Dictionary<OrderStatus, int> StatusCounts { get; set; } = new();
    }
}