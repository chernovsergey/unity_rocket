using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    private float speed = 30f;

    private void Awake() {
        transform.position = new Vector3(
            Random.Range(-25, 25),
            100
        );
    }

    private void OnTriggerEnter2D(Collider2D other) {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void FixedUpdate() {
        transform.position += new Vector3(0, -1, 0) * speed * Time.deltaTime;
        if(transform.position.y < -50) {
            Destroy(gameObject);
        }
        transform.RotateAround(transform.position, new Vector3(0, 0, 1), 5f);
    }

    public void SetSpeed(float x){
        speed = Mathf.Max(30f, Mathf.Min(50f, speed * x));
    }
}
