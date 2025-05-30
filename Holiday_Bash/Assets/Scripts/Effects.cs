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
    public List<AbstractEffect> effectsList = new List<AbstractEffect>();
    /// <summary>
    /// The level of the effect. Higher Levels mean higher effects.
    /// </summary>
    public Effects()
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
        // this.name = name;
        // this.power = power;
        // this.time = time;

        // this.value = libaryEntry.value;
        // this.description = libaryEntry.description;
        // this.frequency = libaryEntry.frequency;
        effectsList = new List<AbstractEffect>();
    }
    public void addEffect(string name, int power, int time)
    {
        effectsList.Add(EffectLibrary.getEffect(name, power, time));
    }

    public void UpdateEffects(object afflictedBeing)
    {
        if (afflictedBeing.GetType() != typeof(AbstractEnemy) && afflictedBeing.GetType() != typeof(Player))
        {
            Debug.Log("effect typign went wrong here");
            return;
        }
        foreach (var key in EffectLibrary.Library)
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

