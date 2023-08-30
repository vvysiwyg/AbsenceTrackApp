namespace WebApiTest.Models
{
    public class Timesheet
    {
        public int Id { get; set; }
        public byte Reason { get; set; }
        public DateTime StartDate { get; set; }
        public byte Duration { get; set; }
        public bool Discounted { get; set; }
        public string Description { get; set; } = "";
    }
}
