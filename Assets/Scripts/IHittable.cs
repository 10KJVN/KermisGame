using UnityEngine;

public interface IHittable
{
    void OnHit(Collision collision, Ball ball);
}