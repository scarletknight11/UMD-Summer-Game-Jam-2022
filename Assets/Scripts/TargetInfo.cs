using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TargetInfo  {

    //If a target in 
    public static bool IsTargetInRange(Vector3 position, Vector3 rayDirection, out RaycastHit HitInfo, float range, LayerMask mask)
    {
        return (Physics.Raycast(position, rayDirection, out HitInfo, range, mask));
    }
}
