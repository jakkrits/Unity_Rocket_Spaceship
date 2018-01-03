using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 1500f;
    [SerializeField] float loadingTime = 1.5f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] ParticleSystem successParticle;


    enum GameState { Alive, Dying, Transcending };
    GameState gameState = GameState.Alive;

    [SerializeField] Boolean collisionsDisabled = false;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (gameState == GameState.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) {
            RespondToDebugKeys();
        }
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        } else {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) { // if no thrusting sound is currently playing
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticle.Play();
    }

    private void RespondToRotateInput() {
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

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        } else if (Input.GetKeyDown(KeyCode.C)) {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (gameState != GameState.Alive || collisionsDisabled) {
            return;
        }
        switch (collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                gameState = GameState.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(success);
                successParticle.Play();
                Invoke("LoadNextLevel", loadingTime);
                break;
            default:
                gameState = GameState.Dying;
                Invoke("LoadFirstLevel", loadingTime);
                audioSource.Stop();
                audioSource.PlayOneShot(death);
                deathParticle.Play();
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