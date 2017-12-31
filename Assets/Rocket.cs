using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] float loadingTime = 3f;

    enum GameState { Alive, Dying, Transcending };
    GameState gameState;
    gameState = GameState.Alive;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (gameState == GameState.Alive) {
            Thrust();
            Rotate();
        }
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space)) {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
        } else {
            audioSource.Stop();
        }
    }

    private void Rotate() {
        rigidBody.freezeRotation = true; // Manual rotation control

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // Resume Physics rotation controls
    }

    private void OnCollisionEnter(Collision collision) {
        if (gameState != GameState.Alive) {
            return;
        }
        switch (collision.gameObject.tag) {
            case "Friendly":
                print("Friendly");
                break;
            case "Finish":
                gameState = GameState.Transcending;
                Invoke("LoadNextLevel", loadingTime);
                break;
            default:
                gameState = GameState.Dying;
                Invoke("LoadFirstLevel", loadingTime);
                print("Dead");
                break;
        }
    }

    private void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel() {
        SceneManager.LoadScene(1);
    }
}