namespace HW02.BussinessContext.DB.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int CategoryId { get; set; }
    }
}
