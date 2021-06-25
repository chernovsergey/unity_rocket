using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private float speed = 10f;

    private void Awake() {
        transform.position = new Vector3(
            Random.Range(-10, 10),
            100
        );
    }

    private void FixedUpdate() {
        // transform.RotateAround(transform.position, new Vector3(0, 0, 1), 1f);
        transform.position += new Vector3(0, -1, 0) * speed * Time.deltaTime;
        if(transform.position.y < -100) {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float x){
        speed = Mathf.Max(10f, Mathf.Min(50f, speed * x));
    }
}
