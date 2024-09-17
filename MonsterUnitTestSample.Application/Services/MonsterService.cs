using System.Security.Cryptography;
using System.Text;
using MonsterUnitTestSample.Application.Models;

namespace MonsterUnitTestSample.Application.Services {
    public class MonsterService {

        public List<Monster> Monsters { get; set; } = new List<Monster>();

        public MonsterService() {

        }

        public Monster GetMonster(string name, int power) {
            var monster = new Monster(name, power);
            Monsters.Add(monster);
            return monster;
        }

        public Monster GetMonsterById(Guid id) {
            return Monsters.FirstOrDefault(m => m.MonsterId == id);
        }

        public MonsterAffliction DamageMonster(Monster attacker, Monster reciever) {
            
            var affliction = new MonsterAffliction {
                Provocator = attacker,
                Afflicted = reciever,
                AfflictionType = MonsterAfflictionType.Damaged,
                Value = attacker.Power,
            };

            affliction.Key = GetAfflictionKey(affliction);
            affliction.Afflicted.Afflictions.Add(affliction);
            return affliction;
        }

        public MonsterAffliction HealMonster(Monster healer, Monster reciever) {
            var affliction = new MonsterAffliction {
                Provocator = healer,
                Afflicted = reciever,
                AfflictionType = MonsterAfflictionType.Healed,
                Value = healer.Power,
            };

            affliction.Key = GetAfflictionKey(affliction);
            affliction.Afflicted.Afflictions.Add(affliction);
            return affliction;
        }

        public int GetCurrentHealth(Monster monster) {
            var health = monster.TotalHealth;
            var damage = monster.Afflictions.Where(a => a.Afflicted.MonsterId == monster.MonsterId && a.AfflictionType == MonsterAfflictionType.Damaged).Sum(a => a.Value);
            var healing = monster.Afflictions.Where(a => a.Afflicted.MonsterId == monster.MonsterId && a.AfflictionType == MonsterAfflictionType.Healed).Sum(a => a.Value);
            
            
            var total = health - damage + healing;
            return total < 0 ? 0 : total > health ? health : total;
        }   

        public bool IsMonsterDead(Monster monster) {
            return GetCurrentHealth(monster) == 0;
        }

        public bool IsAfflictionKeyValid(MonsterAffliction affliction) {
            return affliction.Key == GetAfflictionKey(affliction);
        }

        private string GetAfflictionKey(MonsterAffliction affliction) {
            return GetHash(affliction.Provocator.MonsterId.ToString() + affliction.Afflicted.MonsterId.ToString() + affliction.MonsterAfflictionId);
        }

        private string GetHash(string input) {
            using (var hasher = SHA384.Create()) {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = hasher.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}