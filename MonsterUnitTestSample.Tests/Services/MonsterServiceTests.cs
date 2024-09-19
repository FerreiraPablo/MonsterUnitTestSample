csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using FluentAssertions;
using MonsterUnitTestSample.Application.Models;
using MonsterUnitTestSample.Application.Services;

namespace MonsterUnitTestSample.Tests.Services
{
    public class MonsterServiceTests
    {
        [Fact]
        public void GetMonster_ShouldCreateAndAddMonster()
        {
            // Arrange
            var service = new MonsterService();

            // Act
            var monster = service.GetMonster("TestMonster", 100);

            // Assert
            monster.Should().NotBeNull();
            monster.Name.Should().Be("TestMonster");
            monster.Power.Should().Be(100);
            service.Monsters.Should().Contain(monster);
        }

        [Fact]
        public void GetMonsterById_ShouldReturnCorrectMonster()
        {
            // Arrange
            var service = new MonsterService();
            var monster = service.GetMonster("TestMonster", 100);

            // Act
            var result = service.GetMonsterById(monster.MonsterId);

            // Assert
            result.Should().Be(monster);
        }

        [Fact]
        public void DamageMonster_ShouldCreateCorrectAffliction()
        {
            // Arrange
            var service = new MonsterService();
            var attacker = service.GetMonster("Attacker", 50);
            var receiver = service.GetMonster("Receiver", 100);

            // Act
            var affliction = service.DamageMonster(attacker, receiver);

            // Assert
            affliction.Should().NotBeNull();
            affliction.Provocator.Should().Be(attacker);
            affliction.Afflicted.Should().Be(receiver);
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Damaged);
            affliction.Value.Should().Be(50);
            receiver.Afflictions.Should().Contain(affliction);
        }

        [Fact]
        public void HealMonster_ShouldCreateCorrectAffliction()
        {
            // Arrange
            var service = new MonsterService();
            var healer = service.GetMonster("Healer", 30);
            var receiver = service.GetMonster("Receiver", 100);

            // Act
            var affliction = service.HealMonster(healer, receiver);

            // Assert
            affliction.Should().NotBeNull();
            affliction.Provocator.Should().Be(healer);
            affliction.Afflicted.Should().Be(receiver);
            affliction.AfflictionType.Should().Be(MonsterAfflictionType.Healed);
            affliction.Value.Should().Be(30);
            receiver.Afflictions.Should().Contain(affliction);
        }

        [Fact]
        public void GetCurrentHealth_ShouldCalculateCorrectly()
        {
            // Arrange
            var service = new MonsterService();
            var monster = service.GetMonster("TestMonster", 100);
            monster.TotalHealth = 200;
            var attacker = service.GetMonster("Attacker", 50);
            var healer = service.GetMonster("Healer", 30);

            service.DamageMonster(attacker, monster);
            service.HealMonster(healer, monster);

            // Act
            var health = service.GetCurrentHealth(monster);

            // Assert
            health.Should().Be(180); // 200 - 50 + 30
        }

        [Fact]
        public void IsMonsterDead_ShouldReturnTrueWhenHealthIsZero()
        {
            // Arrange
            var service = new MonsterService();
            var monster = service.GetMonster("TestMonster", 100);
            monster.TotalHealth = 100;
            var attacker = service.GetMonster("Attacker", 100);

            service.DamageMonster(attacker, monster);

            // Act
            var isDead = service.IsMonsterDead(monster);

            // Assert
            isDead.Should().BeTrue();
        }

        [Fact]
        public void IsAfflictionKeyValid_ShouldReturnTrueForValidKey()
        {
            // Arrange
            var service = new MonsterService();
            var attacker = service.GetMonster("Attacker", 50);
            var receiver = service.GetMonster("Receiver", 100);
            var affliction = service.DamageMonster(attacker, receiver);

            // Act
            var isValid = service.IsAfflictionKeyValid(affliction);

            // Assert
            isValid.Should().BeTrue();
        }
    }
}