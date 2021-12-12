namespace DexieWrapper.Persons
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
    }
}
