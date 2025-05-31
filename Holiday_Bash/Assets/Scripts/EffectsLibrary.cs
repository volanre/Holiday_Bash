using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class EffectLibrary
{
    /// <summary>
    /// List of all names of effects
    /// </summary>
    public static List<string> Library = new List<string>()
    {
        "burning",
        "poison"
    };
    public static AbstractEffect getEffect(string name, int power, float time)
    {
        if (name.Equals("burning"))
        {
            return new BurningEffect(power, time);
        }
        else if (name.Equals("poison"))
        {
            return new PoisonEffect(power, time);
        }
        else
        {
            return null;
        }
    }
}
/// <summary>
/// <para>
///     Passive effects are like debuffs and stack ontop of each other.
/// </para>
/// <para>
/// Active effects are applyed once per each effect type at the given frequency.
/// </para>
/// </summary>
public abstract class AbstractEffect
{
    public string name;
    /// <summary>
    /// The level of the effect. Higher Levels mean higher effects.
    /// </summary>
    public int power;
    public float timeRemaining;
    protected int damageValue;


    
    
    protected float effectValue;
    public float frequency;
    public string description;

    /// <summary>
    /// <para>For all multipliers: </para>
    /// <para>If bool is true then it is an MULTIPLICATIVE bonus </para>
    /// If bool is false then it is a ADDITIVE bonus
    /// </summary>
    public Tuple<bool, float> effectMultiplier;
    protected bool effectInPlace = false;
    public AbstractEffect(int power, float time)
    {
        this.power = power;
        this.timeRemaining = time;
    }
    public abstract void DoActiveEffect(AbstractCharacter character, int power);
    public abstract void DoPassiveEffect(AbstractCharacter character);
    public abstract void RemoveStatusEffect(AbstractCharacter character);
}

public class BurningEffect : AbstractEffect
{
    
    public BurningEffect(int power, float time) : base(power, time)
    {
        this.name = "burning";
        this.damageValue = 5;
        this.frequency = 0.5f;
        this.description = "deals continuous fire damage";
    }

    public override void DoActiveEffect(AbstractCharacter character, int power)
    {
        character.TakeDamage(damageValue * power, true);
        return;
    }

    public override void DoPassiveEffect(AbstractCharacter character)
    {
        return;
    }

    public override void RemoveStatusEffect(AbstractCharacter character)
    {
        return;
    }
}
public class PoisonEffect : AbstractEffect
{
    //Poison debuff is additive
    public PoisonEffect(int power, float time) : base(power, time)
    {
        this.name = "poison";
        this.damageValue = 3;
        this.effectValue = 10f;
        this.frequency = 0.5f;
        this.description = "deals continuous poison damage and weakens enemy defense";
        this.effectMultiplier = new Tuple<bool, float>(false, effectValue*power);
        this.effectInPlace = false;
    }

    public override void DoActiveEffect(AbstractCharacter character, int power)
    {
        character.TakeDamage(damageValue * power, true);
        return;
    }

    public override void DoPassiveEffect(AbstractCharacter character)
    {
        if (effectInPlace) return;
        effectInPlace = true;
        character.defenseMultipliers.Add(effectMultiplier);
        return;
    }
    /// <summary>
    /// Removes any additional status effects the player may have had.
    /// </summary>
    /// <param name="character"></param>
    public override void RemoveStatusEffect(AbstractCharacter character)
    {
        if (!effectInPlace) return;
        character.defenseMultipliers.Remove(effectMultiplier);
        effectInPlace = false;

    }
}