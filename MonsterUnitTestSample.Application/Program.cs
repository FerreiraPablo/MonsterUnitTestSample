using MonsterUnitTestSample.Application.Models;
using MonsterUnitTestSample.Application.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var monsterService = new MonsterService();
        var weakMonster = monsterService.GetMonster("Weak", 25);
        var healerMonster = monsterService.GetMonster("Healer", 10);
        var strongMonster = monsterService.GetMonster("Strong", 110);

        PrintMonsterAffliction(monsterService.DamageMonster(weakMonster, strongMonster));

        PrintMonsterAffliction(monsterService.DamageMonster(weakMonster, strongMonster));

        PrintMonsterAffliction(monsterService.HealMonster(healerMonster, strongMonster));

        PrintMonsterAffliction(monsterService.DamageMonster(strongMonster, weakMonster));

        PrintMonsterAffliction(monsterService.DamageMonster(strongMonster, healerMonster));

        Console.WriteLine($"Strong Monster Health: {monsterService.GetCurrentHealth(strongMonster)}");
        
        Console.WriteLine($"Weak Monster Health: {monsterService.GetCurrentHealth(weakMonster)}");

        Console.WriteLine($"Healer Monster Health: {monsterService.GetCurrentHealth(weakMonster)}");
    }

    private static void PrintMonster(Monster monster)
    {
        Console.WriteLine($"Monster: {monster.Name}");
        Console.WriteLine($"Health: {monster.TotalHealth}");
        Console.WriteLine($"Power: {monster.Power}");
        Console.WriteLine();
    }

    private static void PrintMonsterAffliction(MonsterAffliction monsterAffliction) {
        var afflictionType = monsterAffliction.AfflictionType == MonsterAfflictionType.Damaged ? "damaged" : "healed";
        Console.WriteLine($"{monsterAffliction.Provocator.Name} {afflictionType} {monsterAffliction.Afflicted.Name} by {monsterAffliction.Value} points.");
    }
}