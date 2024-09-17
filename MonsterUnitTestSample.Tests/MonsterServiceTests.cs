csharp
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;
using MonsterUnitTestSample.Application.Models;
using MonsterUnitTestSample.Application.Services;

namespace MonsterUnitTestSample.Tests {
    public class MonsterServiceTests {
        private readonly MonsterService _monsterService;

        public MonsterServiceTests() {
            _monsterService = new MonsterService();
        }

        [Fact]
        public void GetMonster_ValidNameAndPower_ShouldCreateMonster() {
            // Arrange
            string name = "Goblin";
            int power = 10;

            // Act
            var monster = _monsterService.GetMonster(name, power);

            // Assert
            monster.Should().NotBeNull();
            monster.Name.Should().Be(name);
            monster.Power.Should().Be(power);
            _monsterService.Monsters.Should().Contain(monster);
        }

        [Fact]
        public void GetMonster_EmptyNameAndZeroPower_ShouldCreateMonster() {
            // Arrange
            string name = "";
            int power = 0;

            // Act
            var monster = _monsterService.GetMonster(name, power);

            // Assert
            monster.Should().NotBeNull();
            monster.Name.Should().Be(name);
            monster.Power.Should().Be(power);
            _monsterService.Monsters.Should().Contain(monster);
        }

        [Fact]
        public void GetMonsterById_ValidId_ShouldReturnMonster() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            var id = monster.MonsterId;

            // Act
            var result = _monsterService.GetMonsterById(id);

            // Assert
            result.Should().Be(monster);
        }

        [Fact]
        public void GetMonsterById_NonExistentId_ShouldReturnNull() {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _monsterService.GetMonsterById(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DamageMonster_ValidAttackerAndReceiver_ShouldApplyDamage() {
            // Arrange
            var attacker = _monsterService.GetMonster("Goblin", 10);
            var receiver = _monsterService.GetMonster("Orc", 20);

            // Act
            var affliction = _monsterService.DamageMonster(attacker, receiver);

            // Assert
            affliction.Should().NotBeNull();
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Damaged);
            affliction.Value.Should().Be(attacker.Power);
            receiver.Afflictions.Should().Contain(affliction);
        }

        [Fact]
        public void DamageMonster_NullAttacker_ShouldThrowException() {
            // Arrange
            Monster attacker = null;
            var receiver = _monsterService.GetMonster("Orc", 20);

            // Act
            Action act = () => _monsterService.DamageMonster(attacker, receiver);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void HealMonster_ValidHealerAndReceiver_ShouldApplyHealing() {
            // Arrange
            var healer = _monsterService.GetMonster("Mage", 15);
            var receiver = _monsterService.GetMonster("Goblin", 10);

            // Act
            var affliction = _monsterService.HealMonster(healer, receiver);

            // Assert
            affliction.Should().NotBeNull();
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Healed);
            affliction.Value.Should().Be(healer.Power);
            receiver.Afflictions.Should().Contain(affliction);
        }

        [Fact]
        public void HealMonster_NullHealer_ShouldThrowException() {
            // Arrange
            Monster healer = null;
            var receiver = _monsterService.GetMonster("Goblin", 10);

            // Act
            Action act = () => _monsterService.HealMonster(healer, receiver);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetCurrentHealth_MonsterWithAfflictions_ShouldCalculateHealthCorrectly() {
            // Arrange
            var monster = _monsterService.GetMonster("Dragon", 30);
            var attacker = _monsterService.GetMonster("Knight", 20);
            _monsterService.DamageMonster(attacker, monster);
            _monsterService.HealMonster(attacker, monster);

            // Act
            var currentHealth = _monsterService.GetCurrentHealth(monster);

            // Assert
            currentHealth.Should().Be(monster.TotalHealth);
        }

        [Fact]
        public void GetCurrentHealth_MonsterWithNoAfflictions_ShouldReturnTotalHealth() {
            // Arrange
            var monster = _monsterService.GetMonster("Dragon", 30);

            // Act
            var currentHealth = _monsterService.GetCurrentHealth(monster);

            // Assert
            currentHealth.Should().Be(monster.TotalHealth);
        }

        [Fact]
        public void IsMonsterDead_MonsterWithZeroHealth_ShouldReturnTrue() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            var attacker = _monsterService.GetMonster("Orc", 15);
            _monsterService.DamageMonster(attacker, monster);

            // Act
            var isDead = _monsterService.IsMonsterDead(monster);

            // Assert
            isDead.Should().BeTrue();
        }

        [Fact]
        public void IsMonsterDead_MonsterWithNonZeroHealth_ShouldReturnFalse() {
            // Arrange
            var monster = _monsterService.GetMonster("Dragon", 30);
            var attacker = _monsterService.GetMonster("Knight", 20);
            _monsterService.DamageMonster(attacker, monster);

            // Act
            var isDead = _monsterService.IsMonsterDead(monster);

            // Assert
            isDead.Should().BeFalse();
        }

        [Fact]
        public void IsAfflictionKeyValid_ValidAfflictionKey_ShouldReturnTrue() {
            // Arrange
            var attacker = _monsterService.GetMonster("Goblin", 10);
            var receiver = _monsterService.GetMonster("Orc", 20);
            var affliction = _monsterService.DamageMonster(attacker, receiver);

            // Act
            var isValid = _monsterService.IsAfflictionKeyValid(affliction);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsAfflictionKeyValid_TamperedAfflictionKey_ShouldReturnFalse() {
            // Arrange
            var attacker = _monsterService.GetMonster("Goblin", 10);
            var receiver = _monsterService.GetMonster("Orc", 20);
            var affliction = _monsterService.DamageMonster(attacker, receiver);
            affliction.Key = "tampered_key";

            // Act
            var isValid = _monsterService.IsAfflictionKeyValid(affliction);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}