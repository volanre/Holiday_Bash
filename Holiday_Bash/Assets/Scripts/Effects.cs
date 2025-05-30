using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Rendering;
using UnityEngine;

public class Effects
{
    
    public string name;
    /// <summary>
    /// The level of the effect. Higher Levels mean higher effects.
    /// </summary>
    public int power;
    public int time;
    private int value;
    private float frequency;
    
    public string description;

    public Effects(string name, int power, int time)
    {
        // Effects libaryEntry;
        // if (!EffectLibrary.library.ContainsKey(name))
        // {
        //     throw new Exception("Dictionary Does Not Contain This Effect Name");
        // }
        // else
        // {
        //     libaryEntry = EffectLibrary.library[name];
        // }
        this.name = name;
        this.power = power;
        this.time = time;

        // this.value = libaryEntry.value;
        // this.description = libaryEntry.description;
        // this.frequency = libaryEntry.frequency;
    }

    public static void UpdateEffects(List<Effects> effectsList, object afflictedBeing)
    {
        if (afflictedBeing.GetType() != typeof(AbstractEnemy) && afflictedBeing.GetType() != typeof(Player))
        {
            Debug.Log("effect typign went wrong here");
            return;
        }
        foreach (var key in EffectLibrary.library.Keys)
        {
            // var duplicatesList = Enumerable.Range(0, effectsList.Count)
            //     .Where(i => effectsList[i].name == key)
            //     .ToList();
            var duplicatesList = effectsList.Where(i => i.name == key).ToList();
            var powersList = duplicatesList.Select(v => v.power).ToList();
            int max = powersList.Max();
        }

    }
}

public static class EffectLibrary
{
    /// <summary>
    /// A Collections of all effects in the game
    /// <para>Key: Name</para>
    /// <para>Value: Description</para>
    /// </summary>
    public static Dictionary<string, AbstractEffect> library = new Dictionary<string, AbstractEffect>
    {
        {"burning", new BurningEffect("burning", 1, 1, 5, 0.5f, "deals continuous damage")}
        // {"poison", new Effects("burning", 1, 1, 5, 0.5f, "deals continuous damage")},
        // {"regeneration", new Effects("burning", 1, 1, 5, 0.5f, "deals continuous damage")},
        // { "poison", "deals continuous damage and lowers target's attack"},
        // {"regeneration", "heals target over time"}
    };
}
public abstract class AbstractEffect
{
    public string name;
    /// <summary>
    /// The level of the effect. Higher Levels mean higher effects.
    /// </summary>
    public int power;
    public int time;
    private int value;
    private float frequency;
    public string description;
    public AbstractEffect(string name, int power, int time, int value, float frequency, string description)
    {
        this.name = name;
        this.power = power;
        this.time = time;
        this.value = value;
        this.description = description;
        this.frequency = frequency;
    }
    public abstract void DoEffect();
}

public class BurningEffect : AbstractEffect
{
    public BurningEffect(string name, int power, int time, int value, float frequency, string description) : base(name, power, time, value, frequency, description)
    {
    }

    public override void DoEffect()
    {
        return;
    }
}