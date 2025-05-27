using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //public static List<AbstractEnemy> enemyTypes = new List<AbstractEnemy>();

    [SerializeField]

    public List<AbstractEnemy> enemyTypes = new List<AbstractEnemy>();

    [SerializeField] public Player player;

    internal AbstractEnemy CreateBaddie(Vector2Int position)
    {
        Vector3 newPos = new Vector3(position.x, position.y, 0);
        var baddie = Instantiate(enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)], newPos, Quaternion.identity);
        baddie.player = player;
        return baddie;
    }

    

}
