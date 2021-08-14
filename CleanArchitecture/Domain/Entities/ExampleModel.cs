namespace Domain.Entities
{
    public class ExampleModel : BaseEntity
    {
        public ExampleModel()
        {

        }
        public ExampleModel(string name, string email)
        {
            Name = name;
            Email = email;
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
