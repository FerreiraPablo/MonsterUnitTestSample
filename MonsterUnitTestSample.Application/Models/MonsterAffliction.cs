namespace MonsterUnitTestSample.Application.Models {
    public class MonsterAffliction {
        public Guid MonsterAfflictionId { get; set; } = Guid.NewGuid();

        public Monster Provocator { get; set; }

        public Monster Afflicted { get; set; }

        public MonsterAfflictionType AfflictionType { get; set; }

        public int Value { get; set; }

        public string Key { get; set; }
    }
}