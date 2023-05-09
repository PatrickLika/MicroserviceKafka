namespace StateMachine.Ioc
{
    public class StateMachineDto
    {
        public string Id { get; set; }
        public int Screws { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public int Price { get; set; }
        public string Cvr { get; set; }
        //public bool IsValid { get; set; }
        public string State { get; set; }
        public string StatePrevious { get; set; }
    }
}



