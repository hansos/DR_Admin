namespace ISPAdmin.Data.Entities
{
    public interface IEntityBase
    {
        int Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
