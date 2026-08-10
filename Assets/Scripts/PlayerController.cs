using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float thrustForce = 1f;
    public float maxSpeed = 10f;
    public InputAction moveForward;
    public InputAction lookPosition;
    private bool isMoving;
    private Vector2 moveDirection;

    public GameObject boosterFlame;

    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;
    public UIDocument uiDocument;
    private Label scoreText;

    public GameObject explosionEffect;
    private Button restartButton;
    private Label highScoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveForward.Enable();
        lookPosition.Enable();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        restartButton.clicked += ReloadScene;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();

        // Check for mouse click
        if (moveForward.IsPressed())
        {
            // Calculate mouse/heading direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(lookPosition.ReadValue<Vector2>());
            moveDirection = ((Vector2)mousePos - (Vector2)transform.position).normalized;

            // Rotate and move the player toward the mouse
            transform.up = moveDirection;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Add booster flame when moving
        if (moveForward.WasPressedThisFrame())
        {
            boosterFlame.SetActive(true);
        }
        else if (moveForward.WasReleasedThisFrame())
        {
            boosterFlame.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void UpdateScore()
    {
        // Track how long the player has been alive and set score
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;
    }

    public float GetScore()
    {
        return score;
    }

    void MovePlayer()
    {
        // Apply movement to the player
        if (isMoving)
        {
            rb.AddForce(moveDirection * thrustForce);

            // Clamp the player's speed
            float clampedVelocity = Mathf.Clamp(rb.linearVelocity.magnitude, 0, maxSpeed);
            rb.linearVelocity = rb.linearVelocity.normalized * clampedVelocity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // The player dies on collision with anything and explodes
        Vector2 pointOfContact = collision.GetContact(0).point;
        Quaternion collisionDirection = Quaternion.LookRotation((Vector2)transform.position - pointOfContact, Vector3.forward);
        Instantiate(explosionEffect, pointOfContact, collisionDirection);
        Destroy(gameObject);
        DisplayGameOverUI();
    }

    void DisplayGameOverUI()
    {
        float highScore = PlayerPrefs.GetFloat("highscore");
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("highscore", highScore);
            highScoreText.text = "High Score: " + Mathf.FloorToInt(highScore);
            highScoreText.style.display = DisplayStyle.Flex;
            restartButton.style.display = DisplayStyle.Flex;
            uiDocument.StartCoroutine(HighscoreFlashRoutine());
        }
        else
        {
            highScoreText.text = "High Score: " + Mathf.FloorToInt(highScore);
            highScoreText.style.display = DisplayStyle.Flex;
            restartButton.style.display = DisplayStyle.Flex;
        }
    }

    IEnumerator HighscoreFlashRoutine()
    {
        Color highscoreBaseColor = highScoreText.resolvedStyle.color;
        Color highscoreFlashColor = new Color32(255, 255, 50, 255);

        int flashes = 0;
        do
        {
            if (flashes == 0)
            {
                yield return new WaitForSeconds(0.33f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }       
            highScoreText.style.color = highscoreFlashColor;
            yield return new WaitForSeconds(0.5f);
            highScoreText.style.color = highscoreBaseColor;
            flashes++;
        } while (flashes < 4);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
