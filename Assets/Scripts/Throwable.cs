using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] private Ball ball;
    //[SerializeField] private GameObject throwablePrefab;

    private float _holdDownStartTime;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Mouse Down, start holding
            _holdDownStartTime = Time.time;
            Debug.Log("Button down!");
        }
        
        if (Input.GetMouseButton(0))
        {
            // Mouse still down, show force
            float holdDownTime = Time.time - _holdDownStartTime;
            ball.ShowForce(CalculateHoldDownForce(holdDownTime));
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            // Mouse Up, Launch!
            float holdDownTime = Time.time - _holdDownStartTime;
            ball.Launch(CalculateHoldDownForce(holdDownTime));
            Debug.Log("Button up!");
        }
    }

    private float CalculateHoldDownForce(float holdTime)
    {
        float maxForceHoldDownTime = 2f;
        //float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float holdTimeNormalized = Mathf.PingPong(holdTime, Mathf.Clamp01(holdTime) / maxForceHoldDownTime);
        
        // if (holdTime >= maxForceHoldDownTime)
        // {
        //     
        // }
        
        float force = holdTimeNormalized * Ball.MaxForce;
        return force;
    }
}
