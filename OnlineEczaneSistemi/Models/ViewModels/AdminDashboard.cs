namespace OnlineEczaneSistemi.Models.ViewModels
{
    public class AdminDashboard
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int PassiveUsers { get; set; }
        public int AdminCount { get; set; }
        public int AdminRoleCount { get; set; }
        public int UserRoleCount { get; set; }
        public List<User> LastUsers { get; set; } = new();

        public List<DailyRegisterStats> Last7DaysStats { get; set; } = new();
    }

    public class DailyRegisterStats
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

}
