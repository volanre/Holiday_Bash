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
    public float burningTimer = 0;
    public Dictionary<string, float> effectTimers = new Dictionary<string, float>();

    public AbstractCharacter character;
    /// <summary>
    /// The level of the effect. Higher Levels mean higher effects.
    /// </summary>
    public Effects(AbstractCharacter character)
    {
        this.character = character;

        effectsList = new List<AbstractEffect>();
        burningTimer = 0;
        effectTimers = new Dictionary<string, float>();
        foreach (var key in EffectLibrary.Library)
        {
            effectTimers.Add(key, 0f);
        }
    }
    public void addEffect(string name, int power, float time)
    {
        effectsList.Add(EffectLibrary.getEffect(name, power, time));
    }

    private void UpdateTimers()
    {
        foreach (var key in EffectLibrary.Library)
        {
            effectTimers[key] += Time.deltaTime;
        }
    }

    public void UpdateEffects()
    {
        UpdateTimers();
        for(int i = 0; i < effectsList.Count; i++)
        {
            var effect = effectsList[i];
            effect.timeRemaining -= Time.deltaTime;
            if (effect.timeRemaining <= 0)
            {
                effect.RemoveStatusEffect(character);
                effectsList.RemoveAt(i);
                i--;
            }
        }

        foreach (var key in EffectLibrary.Library)
        {
            var duplicatesList = effectsList.Where(i => i.name == key).ToList();
            if (duplicatesList.Count <= 0)
            {
                continue;
            }

            //Do the active effect if freuqency allows it
            if (effectTimers[key] >= EffectLibrary.getEffect(key, 1, 1).frequency)
            {
                effectTimers[key] = 0;
                var powersList = duplicatesList.Select(v => v.power).ToList();
                int maxPower = powersList.Max();

                duplicatesList[0].DoActiveEffect(character, maxPower);
            }
            //Do the passive effect/debuffs
            foreach (var effectInstance in duplicatesList)
            {
                effectInstance.DoPassiveEffect(character);
            }

            
        }

    }
}

