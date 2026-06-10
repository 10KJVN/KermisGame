using UnityEngine;
using System.Collections.Generic;

public interface IResettable
{
    void ResetTarget();
}

public class ShootingSession : MonoBehaviour
{
    [SerializeField] private List<GameObject> allTargets;

    public void ResetTargets()
    {
        foreach (var targetObj in allTargets)
        {
            var resettable = targetObj.GetComponent<IResettable>();
            if (resettable != null)
                resettable.ResetTarget();
        }
    }
}
