namespace DbFirst.DTO
{
    public class TripDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }
        public ICollection<CountryDTO> Countries { get; set; }
        public ICollection<ClientDTO> Clients { get; set; }
    }
}
