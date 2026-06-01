using UnityEngine;

public class ShootingSession : MonoBehaviour
{
    [SerializeField] private Breakable[] targets;

    public void ResetTargets()
    {
        foreach (Breakable target in targets)
        {
            target.ResetTarget();
        }
    }
}
