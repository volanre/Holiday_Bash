using System;
using System.Collections.Generic;
using UnityEngine;

public static class EffectLibrary
{
    /// <summary>
    /// List of all names of effects
    /// </summary>
    public static List<string> Library = new List<string>()
    {
        "burning"
    };
    public static AbstractEffect getEffect(string name, int power, int time)
    {
        if (name.Equals("burning"))
        {
            return new BurningEffect(power, time);
        }
        else
        {
            return null;
        }
    }
}
public abstract class AbstractEffect
{
    public string name;
    /// <summary>
    /// The level of the effect. Higher Levels mean higher effects.
    /// </summary>
    public int power;
    public float timeRemaining;
    protected int value;
    public float frequency;
    public string description;
    public AbstractEffect(string name, int power, float time, int value, float frequency, string description)
    {
        this.name = name;
        this.power = power;
        this.timeRemaining = time;
        this.value = value;
        this.description = description;
        this.frequency = frequency;
    }
    public AbstractEffect(int power, float time)
    {
        this.power = power;
        this.timeRemaining = time;

    }
    public abstract void DoEffect(AbstractCharacter character, int power);
}

public class BurningEffect : AbstractEffect
{
    public BurningEffect(string name, int power, float time, int value, float frequency, string description) : base(name, power, time, value, frequency, description)
    {
    }
    public BurningEffect(int power, float time) : base(power, time)
    {
        this.name = "burning";
        this.value = 5;
        this.frequency = 0.5f;
        this.description = "deals continuous fire damage";
    }

    public override void DoEffect(AbstractCharacter character, int power)
    {
        character.TakeDamage(value * power);
        return;
    }
}