using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class ObstacleController : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerController;

    public int resetThreshold = 200; 
    private bool resetting = false;
    private AudioSource audioSource;
    public UIDocument uiDocument;
    private Label resetLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        resetLabel = uiDocument.rootVisualElement.Q<Label>("ResetLabel");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.GetScore() > 0 && playerController.GetScore() % resetThreshold == 0 && !resetting)
        {
            resetting = true;
            audioSource.Play();
            resetLabel.RemoveFromClassList("reset-label-hidden");
            resetLabel.style.display = DisplayStyle.Flex;
            resetLabel.AddToClassList("reset-label-visible");
            StartCoroutine(ResetObstacles());
        } 
    }

    IEnumerator ResetObstacles()
    {
        // Initial pause for courtesy
        yield return new WaitForSeconds(2f);

        // Create ordered list of obstacles
        GameObject[] obstacles = new GameObject[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            obstacles[i++] = child.gameObject;
        }

        // Alternate method
        /* 
        Transform[] obstacles = GetComponentsInChildren<Transform>()
            .Where(t => t != transform)
            .ToArray();
        */

        Vector3 centerScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane));
        Array.Sort(obstacles, (ob1, ob2) =>
        {
            Vector2 dir1 = ob1.GetComponent<Obstacle>().homePosition - (Vector2)centerScreen;
            Vector2 dir2 = ob2.GetComponent<Obstacle>().homePosition - (Vector2)centerScreen;
            
            float angle1 = Mathf.Atan2(-dir1.y, dir1.x);
            float angle2 = Mathf.Atan2(-dir2.y, dir2.x);

            return angle1.CompareTo(angle2);
        });

        // Sequentially interpolate obstacles with foreach
        foreach(GameObject obstacle in obstacles)
        {
            yield return StartCoroutine(obstacle.GetComponent<Obstacle>().MoveToStart(obstacles));
        }

        yield return new WaitForSeconds(0.5f);
        
        // Respawn or re-activate obstacles ...
        foreach(GameObject obstacle in obstacles)
        {
            obstacle.GetComponent<Obstacle>().Reactivate();
        }
        

        resetting = false;
        resetLabel.RemoveFromClassList("reset-label-visible");
        resetLabel.style.display = DisplayStyle.None;
        resetLabel.AddToClassList("reset-label-hidden");
    }
}
