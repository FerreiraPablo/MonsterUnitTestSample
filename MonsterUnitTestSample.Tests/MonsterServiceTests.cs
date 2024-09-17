csharp
using System;
using System.Collections.Generic;
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
        public void GetMonsterById_ValidId_ShouldReturnMonster() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            var id = monster.MonsterId;

            // Act
            var result = _monsterService.GetMonsterById(id);

            // Assert
            result.Should().BeEquivalentTo(monster);
        }

        [Fact]
        public void GetMonsterById_InvalidId_ShouldReturnNull() {
            // Act
            var result = _monsterService.GetMonsterById(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DamageMonster_ShouldCreateAfflictionWithCorrectDetails() {
            // Arrange
            var attacker = _monsterService.GetMonster("Goblin", 10);
            var receiver = _monsterService.GetMonster("Orc", 15);

            // Act
            var affliction = _monsterService.DamageMonster(attacker, receiver);

            // Assert
            affliction.Provocator.Should().Be(attacker);
            affliction.Afflicted.Should().Be(receiver);
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Damaged);
            affliction.Value.Should().Be(attacker.Power);
        }

        [Fact]
        public void HealMonster_ShouldCreateAfflictionWithCorrectDetails() {
            // Arrange
            var healer = _monsterService.GetMonster("Elf", 20);
            var receiver = _monsterService.GetMonster("Orc", 15);

            // Act
            var affliction = _monsterService.HealMonster(healer, receiver);

            // Assert
            affliction.Provocator.Should().Be(healer);
            affliction.Afflicted.Should().Be(receiver);
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Healed);
            affliction.Value.Should().Be(healer.Power);
        }

        [Fact]
        public void GetCurrentHealth_NoAfflictions_ShouldReturnTotalHealth() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            monster.TotalHealth = 20;

            // Act
            var health = _monsterService.GetCurrentHealth(monster);

            // Assert
            health.Should().Be(monster.TotalHealth);
        }

        [Fact]
        public void GetCurrentHealth_OnlyDamageAfflictions_ShouldReturnCorrectHealth() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            monster.TotalHealth = 20;
            _monsterService.DamageMonster(monster, monster);

            // Act
            var health = _monsterService.GetCurrentHealth(monster);

            // Assert
            health.Should().Be(monster.TotalHealth - monster.Power);
        }

        [Fact]
        public void GetCurrentHealth_OnlyHealingAfflictions_ShouldReturnCorrectHealth() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            monster.TotalHealth = 20;
            _monsterService.HealMonster(monster, monster);

            // Act
            var health = _monsterService.GetCurrentHealth(monster);

            // Assert
            health.Should().Be(monster.TotalHealth + monster.Power);
        }

        [Fact]
        public void GetCurrentHealth_DamageAndHealingAfflictions_ShouldReturnCorrectHealth() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            monster.TotalHealth = 20;
            _monsterService.DamageMonster(monster, monster);
            _monsterService.HealMonster(monster, monster);

            // Act
            var health = _monsterService.GetCurrentHealth(monster);

            // Assert
            health.Should().Be(monster.TotalHealth);
        }

        [Fact]
        public void GetCurrentHealth_HealthShouldNotExceedTotalHealth() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            monster.TotalHealth = 20;
            _monsterService.HealMonster(monster, monster);
            _monsterService.HealMonster(monster, monster);

            // Act
            var health = _monsterService.GetCurrentHealth(monster);

            // Assert
            health.Should().Be(monster.TotalHealth);
        }

        [Fact]
        public void IsMonsterDead_WhenHealthIsZero_ShouldReturnTrue() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            monster.TotalHealth = 20;
            _monsterService.DamageMonster(monster, monster);

            // Act
            var isDead = _monsterService.IsMonsterDead(monster);

            // Assert
            isDead.Should().BeTrue();
        }

        [Fact]
        public void IsMonsterDead_WhenHealthIsGreaterThanZero_ShouldReturnFalse() {
            // Arrange
            var monster = _monsterService.GetMonster("Goblin", 10);
            monster.TotalHealth = 20;

            // Act
            var isDead = _monsterService.IsMonsterDead(monster);

            // Assert
            isDead.Should().BeFalse();
        }

        [Fact]
        public void IsAfflictionKeyValid_ValidKey_ShouldReturnTrue() {
            // Arrange
            var attacker = _monsterService.GetMonster("Goblin", 10);
            var receiver = _monsterService.GetMonster("Orc", 15);
            var affliction = _monsterService.DamageMonster(attacker, receiver);

            // Act
            var isValid = _monsterService.IsAfflictionKeyValid(affliction);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsAfflictionKeyValid_InvalidKey_ShouldReturnFalse() {
            // Arrange
            var affliction = new MonsterAffliction {
                Key = "invalid_key"
            };

            // Act
            var isValid = _monsterService.IsAfflictionKeyValid(affliction);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}