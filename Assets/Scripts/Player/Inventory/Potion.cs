using System;
using System.Collections.Generic;

public enum PotionType
{
    Small,
    Medium,
    Large,
    ExtraLarge,
    Mega,
    Ultra,
    Gargantuan
}
public class Potion : Consumable
{
    public static readonly Dictionary<PotionType, Tuple<int, int>> HealingPowerDict = new Dictionary<PotionType, Tuple<int, int>>()
            {
                { PotionType.Small, new(2,5) },
                { PotionType.Medium, new(5,10) },
                { PotionType.Large, new(8,15) },
                { PotionType.ExtraLarge, new(12,25) },
                { PotionType.Mega, new(20,35) },
                { PotionType.Ultra, new(30,50) },
                { PotionType.Gargantuan, new(50,100) },
            };
    public int HealingPower;
    public PotionType potionType;
    public Potion(string name, Tuple<int, int> worth, PotionType type) : base(name, worth)
    {
        potionType = type;
        HealingPower = ACG.NumBetween(HealingPowerDict[type]);
    }

    public override string Use()
    {
        int prevHealth = PlayerStats.Health;
        PlayerStats.AddHealth(HealingPower);
        return base.Use() + $" You gained {HealingPower} health!\n<color=red>{prevHealth}</color> -> <color=green>{(PlayerStats.Health == PlayerStats.MaxHealth ? $"{PlayerStats.Health}(MAX)" : PlayerStats.Health)}";
    }

    public static Potion Create(string name, Tuple<int, int> worth, PotionType type) => new Potion(name, worth, type);
}