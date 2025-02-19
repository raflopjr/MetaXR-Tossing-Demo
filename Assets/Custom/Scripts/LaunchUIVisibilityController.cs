using UnityEngine;
using Oculus.Interaction;

public class LaunchUIVisibilityController : MonoBehaviour
{
    GameObject pokeablePanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Hide the launch UI by default.
        pokeablePanel = GameObject.Find("Pokeable Panel");
        if (pokeablePanel != null)
        {
            pokeablePanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        InteractableGroupView groupView = transform.parent.GetComponentInChildren<InteractableGroupView>();
        // If the ball is currently in possession, then show launch UI.
        if(groupView.SelectingInteractorsCount > 0) {
            pokeablePanel.SetActive(true);
        }
        // Otherwise, hide it.
        else {
            pokeablePanel.SetActive(false);
        }
    }
}
