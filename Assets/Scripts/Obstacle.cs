using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    public float minSpeed = 50f;
    public float maxSpeed = 150f;
    public float maxSpinSpeed = 10f;

    Rigidbody2D rb;

    [HideInInspector]
    public Vector2 homePosition;

    public GameObject bounceEffect;
    public float minCollisionSpeed = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Randomize the obstacle's speed, size, and direction, then launch it       
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);
        
        float averageSize = (minSize + maxSize) / 2; 
        float randomSpeed = Random.Range(minSpeed, maxSpeed) * averageSize/randomSize;     
        Vector2 randomDirection = Random.onUnitCircle;
        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(randomDirection * randomSpeed);
        rb.AddTorque(randomTorque);

        // Field assignment
        homePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Spawn a particle effect from the faster obstacle
        Vector2 contactPoint = collision.GetContact(0).point;
        float currentSpeed = rb.linearVelocity.magnitude;
        if ((collision.rigidbody == null || currentSpeed > collision.rigidbody.linearVelocity.magnitude) && currentSpeed > minCollisionSpeed)
        {
            GameObject effect = Instantiate(bounceEffect, contactPoint, Quaternion.identity);
            Destroy(effect, 0.5f);
        }    
    }

    // Interpolate the obstacle back to its starting position
    public IEnumerator MoveToStart(GameObject[] ignoreCollision)
    {
        // Set current obstacle to ignore collisions with other obstacles while moving
        Collider2D collider = GetComponent<Collider2D>();
        foreach (GameObject ob in ignoreCollision)
        {
            if (ob != this)
            {
                Physics2D.IgnoreCollision(collider, ob.GetComponent<Collider2D>(), true);
            }
        }

        Vector2 startPosition = transform.position;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;

        float elapsed = 0f;
        float transitionTime = 1f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionTime);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector2.Lerp(startPosition, homePosition, eased);
            yield return null;
        }

        transform.position = homePosition;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;

        // Set collisions back to normal
        foreach (GameObject ob in ignoreCollision)
        {
            if (ob != this)
            {
                Physics2D.IgnoreCollision(collider, ob.GetComponent<Collider2D>(), false);
            }
        }
    }

    public void Reactivate()
    {
        StopAllCoroutines();
        float averageSize = (minSize + maxSize) / 2; 
        float randomSpeed = Random.Range(minSpeed, maxSpeed) * averageSize/transform.localScale.x;       
        Vector2 randomDirection = Random.onUnitCircle;
        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);

        rb.AddForce(randomDirection * randomSpeed);
        rb.AddTorque(randomTorque);
    }
}
