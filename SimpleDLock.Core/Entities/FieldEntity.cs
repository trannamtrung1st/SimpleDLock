namespace SimpleDLock.Core.Entities
{
    public class FieldEntity
    {
        public FieldEntity()
        {
        }

        public FieldEntity(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
