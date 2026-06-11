using UnityEngine;
using System.Collections.Generic;

public class ShootingSession : MonoBehaviour
{
    [SerializeField] private List<GameObject> allTargets;

    public void ResetTargets()
    {
        foreach (var targetObj in allTargets)
        {
            var resettable = targetObj.GetComponent<IResettable>();
            resettable?.ResetTarget();
        }
    }
}
