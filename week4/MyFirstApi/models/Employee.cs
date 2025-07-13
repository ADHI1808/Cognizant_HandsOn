namespace MyFirstAPI.models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Salary { get; set; }
        public bool Permanent { get; set; }
        public Department Department { get; set; } = new Department();
        public List<Skills> Skills { get; set; } = new List<Skills>();
        public DateTime DateOfBirth { get; set; }
    }
}