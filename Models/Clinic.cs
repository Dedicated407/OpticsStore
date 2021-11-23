namespace OpticsStore.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int FactoryId { get; set; }
        public Factory Factory { get; set; }
    }
}