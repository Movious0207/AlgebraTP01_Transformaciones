using CustomMath;
using UnityEngine;

public class quatest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Quat.AngleAxis(30,Vec3.Up);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
