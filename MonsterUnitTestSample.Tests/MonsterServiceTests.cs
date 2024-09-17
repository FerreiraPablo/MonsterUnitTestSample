csharp
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;
using MonsterUnitTestSample.Application.Models;
using MonsterUnitTestSample.Application.Services;

namespace MonsterUnitTestSample.Tests
{
    public class MonsterServiceTests
    {
        private readonly MonsterService _monsterService;

        public MonsterServiceTests()
        {
            _monsterService = new MonsterService();
        }

        [Fact]
        public void GetMonster_ShouldAddMonster_WhenCalledWithValidParameters()
        {
            // Arrange
            var name = "Goblin";
            var power = 10;

            // Act
            var monster = _monsterService.GetMonster(name, power);

            // Assert
            monster.Should().NotBeNull();
            monster.Name.Should().Be(name);
            monster.Power.Should().Be(power);
            _monsterService.Monsters.Should().Contain(monster);
        }

        [Fact]
        public void GetMonster_ShouldThrowArgumentException_WhenNameIsNull()
        {
            // Arrange
            string name = null;
            var power = 10;

            // Act
            Action act = () => _monsterService.GetMonster(name, power);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or empty.");
        }

        [Fact]
        public void GetMonsterById_ShouldReturnMonster_WhenIdExists()
        {
            // Arrange
            var monster = _monsterService.GetMonster("Orc", 15);
            var id = monster.MonsterId;

            // Act
            var result = _monsterService.GetMonsterById(id);

            // Assert
            result.Should().Be(monster);
        }

        [Fact]
        public void GetMonsterById_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _monsterService.GetMonsterById(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DamageMonster_ShouldApplyDamage_WhenCalledWithValidMonsters()
        {
            // Arrange
            var attacker = new Monster("Dragon", 20);
            var receiver = new Monster("Knight", 15);

            // Act
            var affliction = _monsterService.DamageMonster(attacker, receiver);

            // Assert
            affliction.Should().NotBeNull();
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Damaged);
            affliction.Value.Should().Be(attacker.Power);
            receiver.Afflictions.Should().Contain(affliction);
        }

        [Fact]
        public void DamageMonster_ShouldThrowArgumentNullException_WhenAttackerIsNull()
        {
            // Arrange
            Monster receiver = new Monster("Knight", 15);

            // Act
            Action act = () => _monsterService.DamageMonster(null, receiver);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Attacker cannot be null.");
        }

        [Fact]
        public void HealMonster_ShouldApplyHealing_WhenCalledWithValidMonsters()
        {
            // Arrange
            var healer = new Monster("Cleric", 10);
            var receiver = new Monster("Knight", 15);

            // Act
            var affliction = _monsterService.HealMonster(healer, receiver);

            // Assert
            affliction.Should().NotBeNull();
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Healed);
            affliction.Value.Should().Be(healer.Power);
            receiver.Afflictions.Should().Contain(affliction);
        }

        [Fact]
        public void HealMonster_ShouldThrowArgumentNullException_WhenHealerIsNull()
        {
            // Arrange
            Monster receiver = new Monster("Knight", 15);

            // Act
            Action act = () => _monsterService.HealMonster(null, receiver);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Healer cannot be null.");
        }

        [Fact]
        public void GetCurrentHealth_ShouldCalculateHealthCorrectly_WhenAfflictionsExist()
        {
            // Arrange
            var monster = new Monster("Knight", 20);
            var damageAffliction = _monsterService.DamageMonster(new Monster("Dragon", 15), monster);
            var healAffliction = _monsterService.HealMonster(new Monster("Cleric", 10), monster);

            // Act
            var currentHealth = _monsterService.GetCurrentHealth(monster);

            // Assert
            currentHealth.Should().Be(monster.TotalHealth - damageAffliction.Value + healAffliction.Value);
        }

        [Fact]
        public void IsMonsterDead_ShouldReturnTrue_WhenHealthIsZero()
        {
            // Arrange
            var monster = new Monster("Knight", 20);
            _monsterService.DamageMonster(new Monster("Dragon", 25), monster);

            // Act
            var isDead = _monsterService.IsMonsterDead(monster);

            // Assert
            isDead.Should().BeTrue();
        }

        [Fact]
        public void IsMonsterDead_ShouldReturnFalse_WhenHealthIsAboveZero()
        {
            // Arrange
            var monster = new Monster("Knight", 20);
            _monsterService.DamageMonster(new Monster("Dragon", 15), monster);

            // Act
            var isDead = _monsterService.IsMonsterDead(monster);

            // Assert
            isDead.Should().BeFalse();
        }

        [Fact]
        public void IsAfflictionKeyValid_ShouldReturnTrue_WhenKeyIsValid()
        {
            // Arrange
            var monster1 = new Monster("Dragon", 20);
            var monster2 = new Monster("Knight", 15);
            var affliction = _monsterService.DamageMonster(monster1, monster2);

            // Act
            var isValid = _monsterService.IsAfflictionKeyValid(affliction);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsAfflictionKeyValid_ShouldReturnFalse_WhenKeyIsTampered()
        {
            // Arrange
            var monster1 = new Monster("Dragon", 20);
            var monster2 = new Monster("Knight", 15);
            var affliction = _monsterService.DamageMonster(monster1, monster2);
            affliction.Key = "tampered_key";

            // Act
            var isValid = _monsterService.IsAfflictionKeyValid(affliction);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void MonsterAffliction_ShouldInitializePropertiesCorrectly()
        {
            // Arrange & Act
            var affliction = new MonsterAffliction();

            // Assert
            affliction.MonsterAfflictionId.Should().NotBe(Guid.Empty);
            affliction.Provocator.Should().BeNull();
            affliction.Afflicted.Should().BeNull();
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Damaged);
            affliction.Value.Should().Be(0);
            affliction.Key.Should().BeNull();
        }
    }
}