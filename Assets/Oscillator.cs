using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] // Allow only one component

public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(-20f, 0f, 0f);
    [SerializeField] float period = 2f;

    [SerializeField] [Range(0.1f, 1f)] float movementFactor; // 0 = stationary 1 = fully moved

    Vector3 startingPosition;

    // Use this for initialization
    void Start() {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSineWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSineWave / 2f + .5f;

        Vector3 offset = movementFactor * movementVector;
        transform.position = offset + startingPosition;
    }
}
