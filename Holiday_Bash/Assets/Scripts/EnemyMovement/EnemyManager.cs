using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //public static List<AbstractEnemy> enemyTypes = new List<AbstractEnemy>();

    [SerializeField]
    [Header("Enemies")]
    public List<AbstractEnemy> enemyTypes = new List<AbstractEnemy>();

    [NonSerialized] public Player player;
    [Header("Bosses")]

    [SerializeField] private List<AbstractEnemy> Floor1Bosses = new List<AbstractEnemy>();
    [SerializeField] private Ifrit ifrit;

    internal AbstractEnemy CreateBaddie(Vector2Int position, int FloorOfTheDungeon = 1, bool isBoss = false)
    {
        Vector3 newPos = new Vector3(position.x, position.y, 0);
        AbstractEnemy enemy = getRandomEnemy();
        if (isBoss) enemy = getBoss(FloorOfTheDungeon);
        var baddie = Instantiate(enemy, newPos, Quaternion.identity);
        baddie.player = player;
        return baddie;
    }
    public AbstractEnemy getRandomEnemy() {
        return enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)];
    }
    public AbstractEnemy getBoss(int FloorOfTheDungeon)
    {
        if (FloorOfTheDungeon == 1)
        {
            return Floor1Bosses[UnityEngine.Random.Range(0, Floor1Bosses.Count)];
        }
        else if (FloorOfTheDungeon == 99)
        {
            return ifrit;
        }
        else
        {
            return Floor1Bosses[UnityEngine.Random.Range(0, Floor1Bosses.Count)];
        }
    }

    

}
