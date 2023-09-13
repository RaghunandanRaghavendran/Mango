namespace Mango.Web.Models
{
    public class UpsertCartDetailsDTO
    {
        public int CartId { get; set; }
        public int CartDetailsId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
    }
}
