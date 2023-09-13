namespace Mango.Web.Utility
{
    public class StaticDetails
    {
        public enum ApiType
        {
            GET, POST, PUT, DELETE, PATCH, TRACE
        }

        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
		public static string ProductAPIBase { get; set; }
        public static string CartAPIBase { get; set; }
        public static string OrderAPIBase { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string RoleFrontDesk = "FRONT DESK";
        public const string RoleKitchen = "KITCHEN";
        public const string TOKENCOOKIE = "JWTToken";

        public const string STATUS_PENDING = "Pending";
        public const string STATUS_APPROVED = "Approved";
        public const string STATUS_READYFORPICKUP = "ReadyForPickup";
        public const string STATUS_COMPLETED = "Completed";
        public const string STATUS_REFUNDED = "Refunded";
        public const string STATUS_CANCELLED = "Cancelled";
    }
}
