namespace Api.Dto
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public int Screw { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public int Price { get; set; }
        public string State { get; set; }
        public string Cvr { get; set; }

    }
}
