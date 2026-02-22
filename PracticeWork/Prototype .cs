using System;
using System.Collections.Generic;

public class Weapon : ICloneable
{
    public string Name { get; set; } = "";
    public int Damage { get; set; }

    public object Clone()
    {
        return new Weapon { Name = this.Name, Damage = this.Damage };
    }
}

public class Armor : ICloneable
{
    public string Name { get; set; } = "";
    public int Defense { get; set; }

    public object Clone()
    {
        return new Armor { Name = this.Name, Defense = this.Defense };
    }
}

public class Skill : ICloneable
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = ""; 
    public int Power { get; set; }

    public object Clone()
    {
        return new Skill { Name = this.Name, Type = this.Type, Power = this.Power };
    }
}

public class Character : ICloneable
{
    public int Health { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Intelligence { get; set; }

    public Weapon Weapon { get; set; } = new Weapon();
    public Armor Armor { get; set; } = new Armor();

    public List<Skill> Skills { get; set; } = new List<Skill>();

    public object Clone()
    {
        var copy = new Character
        {
            Health = this.Health,
            Strength = this.Strength,
            Agility = this.Agility,
            Intelligence = this.Intelligence,
            Weapon = (Weapon)this.Weapon.Clone(),
            Armor = (Armor)this.Armor.Clone()
        };

        foreach (var skill in this.Skills)
            copy.Skills.Add((Skill)skill.Clone());

        return copy;
    }

    public void Print(string title)
    {
        Console.WriteLine($"=== {title} ===");
        Console.WriteLine($"HP={Health} STR={Strength} AGI={Agility} INT={Intelligence}");
        Console.WriteLine($"Weapon: {Weapon.Name} dmg={Weapon.Damage}");
        Console.WriteLine($"Armor: {Armor.Name} def={Armor.Defense}");
        Console.WriteLine("Skills:");
        foreach (var s in Skills)
            Console.WriteLine($" - {s.Type}: {s.Name} power={s.Power}");
        Console.WriteLine();
    }
}

public class Program_PrototypePractice
{
    public static void Main()
    {
        var warrior = new Character
        {
            Health = 200,
            Strength = 20,
            Agility = 10,
            Intelligence = 5,
            Weapon = new Weapon { Name = "Sword", Damage = 25 },
            Armor = new Armor { Name = "Iron Armor", Defense = 15 }
        };
        warrior.Skills.Add(new Skill { Type = "Physical", Name = "Slash", Power = 10 });
        warrior.Skills.Add(new Skill { Type = "Physical", Name = "Block", Power = 5 });

        warrior.Print("WARRIOR (prototype)");

        var warrior2 = (Character)warrior.Clone();
        warrior2.Health = 180;
        warrior2.Weapon.Name = "Axe";
        warrior2.Weapon.Damage = 30;
        warrior2.Skills[0].Name = "Heavy Slash";

        warrior2.Print("WARRIOR 2 (clone modified)");

        warrior.Print("WARRIOR (after clone changes)");
    }
}