namespace MonsterUnitTestSample.Application.Models {
    public class Monster {
        public Guid MonsterId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int TotalHealth { get; set; } = 100;
        public int Power { get; set; }
        public List<MonsterAffliction> Afflictions { get; set; } = new List<MonsterAffliction>();

        public Monster(string name, int power) {
            Name = name;
            Power = power;
        }
    }
}