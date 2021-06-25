using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assets : MonoBehaviour
{
    private static Assets instance;

public static Assets GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }

    public Transform pfReward;
    public Transform pfBlackHole;
    public Transform pfAsteroid;
}
