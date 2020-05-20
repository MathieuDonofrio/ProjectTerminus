using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class SearchUtil
{

    public static GameObject FindClosest(GameObject[] gameObjects, Vector3 position)
    {
        GameObject target = null;
        float targetSqrDistance = Mathf.Infinity;

        for(int i = 0; i < gameObjects.Length; i++)
        {
            float distanceSqr = (gameObjects[i].transform.position - position).sqrMagnitude;

            if(distanceSqr < targetSqrDistance)
            {
                target = gameObjects[i];
                targetSqrDistance = distanceSqr;
            }
        }

        return target;
    }
}

