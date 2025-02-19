using UnityEngine;

public class PortalHandler : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        // Get the renderer for this object.
        Renderer thisRenderer = GetComponent<Renderer>();
        // Get the renderer for the colliding object.
        Renderer collidedRenderer = collision.gameObject.GetComponent<Renderer>();
        if (collidedRenderer != null && thisRenderer != null) {
            GameObject gameManager = GameObject.Find("GameManager");
            Debug.Log("Calling gameMaster.PlayerDidScore with collidedRenderer and thisRenderer");
            if (gameManager != null)
            {
                GameMaster gameMaster = gameManager.GetComponent<GameMaster>();
                gameMaster.PlayerDidScore(collidedRenderer, thisRenderer);
            }
        }
    }
}
