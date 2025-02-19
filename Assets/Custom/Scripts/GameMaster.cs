using UnityEngine;
using TMPro;
using Oculus.Interaction;

public class GameMaster : MonoBehaviour
{
    public GameObject gamePiece;
    public GameObject goalPiece;

    public GameObject playerCameraRig;

    public GameObject menu;

    public GameObject scoreUI;

    public Color[] colors = new Color[] {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    public float worldScale = 2.0f;

    // The maximum arc width in degrees to place the goals
    public float maxArcWidth = 360f; 
    // The amount to increase the score by when the player scores.
    public float scoreAmount = 10f;
    // The speed at which the player moves.
    public float movementSpeed = 0.1f;
    // The speed at which the player rotates.
    public float rotateSpeed = 2.0f;
    // The distance the menu will be from the player when it spawns
    public float menuDistance = 0.5f;
    // The minimum distance the goals can be from the player.
    public float minGoalDistance = 3.0f;
    // The maximum distance the goals can be from the player.
    public float maxGoalDistance = 25.0f;
    // The amount to change the goal distance by when the player presses the buttons.
    public float distanceChangeAmount = 1.0f;
    // The force to throw the ball when the player hits the button.
    public float throwingForce = 20.0f;

    // The spawned goals in the scene.
    private GameObject[] spawnedGoals;
    // The active ball in the scene.
    private GameObject ball;

    // Distance from the center of the circle (player position)
    private float goalDistance = 10f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        // Hide the menu when the game starts.
        menu.SetActive(false);

        // Store the spawned goals in an array to reference them later.
        spawnedGoals = new GameObject[colors.Length];
        
        // Create the goals.
        CreateGoals();
        // Create the active ball.
        NewBall();
    }

    void Update() {
        // Use the player current position to orient the score UI to the player.
        Vector3 lookDirection = playerCameraRig.transform.position - scoreUI.transform.position;
        lookDirection.y = 0; // Keep only the horizontal direction
        // The Euler angles are in degrees to correct it being backwards
        scoreUI.transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180, 0);
    }

    // Helper method to help spawn goals.
    void CreateGoals() {
        for (int i = 0; i < spawnedGoals.Length; i++) {
            Destroy(spawnedGoals[i]);
        }

        float angleStep = maxArcWidth / colors.Length;
        for (int i = 0; i < colors.Length; i++) {
            float angle = i * angleStep;
            Vector3 position = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * goalDistance,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * goalDistance
            );

            GameObject goal = Instantiate(goalPiece, position, Quaternion.identity);
            goal.transform.LookAt(Vector3.zero); // Rotate to face the center

            // Select the nested Plane and update its material color
            Transform planeTransform = goal.transform.Find("Plane");
            if (planeTransform != null)
            {
                Renderer ballRenderer = planeTransform.GetComponent<Renderer>();
                ballRenderer.material.SetColor("_BackgroundColor", colors[i]);
            }

            // Select the nested Plane for the backside of the portal.
            Transform planeOneTransform = goal.transform.Find("Plane (1)");
            if (planeOneTransform != null)
            {
                Renderer ballRenderer = planeOneTransform.GetComponent<Renderer>();
                ballRenderer.material.SetColor("_BackgroundColor", colors[i]);
            }

            spawnedGoals[i] = goal;
        }
    }

    // Helper method to disable movement when player is in the menu.
    bool IsMovementDisabled() {
        return menu.activeSelf;
    }

    // Helper method to create a new ball.
    void NewBall() {
        // Create the active ball and randomly assign a color.
        ball = Instantiate(gamePiece, new Vector3(0, 1.5f, 0), Quaternion.identity);
        // Select the nested Sphere and update its material color and disable gravity
        Transform interactable = ball.transform.Find("GamePiece Interactable");
        if (interactable != null) {
            Rigidbody ballRigidbody = interactable.GetComponent<Rigidbody>();
            ballRigidbody.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);

            Renderer ballRenderer = interactable.GetComponent<Renderer>();
            ballRenderer.material.SetColor("_BackgroundColor", colors[Random.Range(0, colors.Length)]);
        }
    }

    // Helper method to handle when the player scores.
    void PlayerScored() {
        // Destroy the ball that scored.
        Destroy(ball);

        Transform scoreText = scoreUI.transform.Find("ScoreData");
        TextMeshProUGUI scoreTextMesh = scoreText.GetComponent<TextMeshProUGUI>();
        if (scoreTextMesh != null)
        {
            float currentScore;
            if (float.TryParse(scoreTextMesh.text, out currentScore))
            {
                currentScore += scoreAmount;
                scoreTextMesh.text = currentScore.ToString();
            }
        }

        // Create a new ball with a random color.
        NewBall();
    }

    public void DecreaseGoalDistance() {
        if(goalDistance - distanceChangeAmount >= minGoalDistance)
            goalDistance -= distanceChangeAmount;
        CreateGoals();
    }

    public void IncreaseGoalDistance() {
        if(goalDistance + distanceChangeAmount <= maxGoalDistance)
            goalDistance += distanceChangeAmount;
        CreateGoals();
    }

    public void MovePlayerForward() {
        if (IsMovementDisabled()) return;

        // From the camera rig find eye anchor to calculate move relative to the player's forward direction.
        Transform trackingSpace = playerCameraRig.transform.Find("TrackingSpace");
        Transform centerEyeAnchor = trackingSpace.transform.Find("CenterEyeAnchor");

        if (centerEyeAnchor != null) {
            // User eye direction to adjust position.
            Vector3 forwardDirection = new Vector3(centerEyeAnchor.transform.forward.x, 0, centerEyeAnchor.transform.forward.z).normalized;
            playerCameraRig.transform.position += forwardDirection * movementSpeed;
        }
    }

    public void MovePlayerBackward() {
        if (IsMovementDisabled()) return;

        // From the camera rig find eye anchor to calculate move relative to the player's forward direction.
        Transform trackingSpace = playerCameraRig.transform.Find("TrackingSpace");
        Transform centerEyeAnchor = trackingSpace.transform.Find("CenterEyeAnchor");

        if (centerEyeAnchor != null) {
            // User eye direction to adjust position.
            Vector3 forwardDirection = new Vector3(centerEyeAnchor.transform.forward.x, 0, centerEyeAnchor.transform.forward.z).normalized;
            // Rotate the forward direction to the opposite direction to move backward.
            Vector3 backwardDirection = Quaternion.Euler(0, 180, 0) * forwardDirection;
            playerCameraRig.transform.position += backwardDirection * movementSpeed;
        }
    }

    public void MovePlayerLeft() {
        if (IsMovementDisabled()) return;

        // From the camera rig find eye anchor to calculate move relative to the player's forward direction.
        Transform trackingSpace = playerCameraRig.transform.Find("TrackingSpace");
        Transform centerEyeAnchor = trackingSpace.transform.Find("CenterEyeAnchor");

        if (centerEyeAnchor != null) {
            // User eye direction to adjust position.
            Vector3 forwardDirection = new Vector3(centerEyeAnchor.transform.forward.x, 0, centerEyeAnchor.transform.forward.z).normalized;
            // Rotate the forward direction to the left to move left.
            Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * forwardDirection;

            playerCameraRig.transform.position += leftDirection * movementSpeed;
        }
    }

    public void MovePlayerRight() {
        if (IsMovementDisabled()) return;

        // From the camera rig find eye anchor to calculate move relative to the player's forward direction.
        Transform trackingSpace = playerCameraRig.transform.Find("TrackingSpace");
        Transform centerEyeAnchor = trackingSpace.transform.Find("CenterEyeAnchor");

        if (centerEyeAnchor != null) {
            // User eye direction to adjust position.
            Vector3 rightDirection = new Vector3(centerEyeAnchor.transform.right.x, 0, centerEyeAnchor.transform.right.z).normalized;
            playerCameraRig.transform.position += rightDirection * movementSpeed;
        }
    }

    public bool PlayerDidScore(Renderer gamePieceRenderer, Renderer goalRenderer) {
        Color colorOfCollison  = gamePieceRenderer.material.GetColor("_BackgroundColor");
        Color goalColor = goalRenderer.material.GetColor("_BackgroundColor");

        Debug.Log("PlayerDidScore called with colorOfCollison: " + colorOfCollison);
        Debug.Log(" and goalColor: " + goalColor);
        
        if (colorOfCollison == goalColor) {
            PlayerScored();
            return true;
        } else {
            return false;
        }
    }

    public void RestartGame() {
        // Reset the goal distance to the default value.
        goalDistance = 10f;

        CreateGoals();

        // Reset the score to 0.
        Transform scoreText = scoreUI.transform.Find("ScoreData");
        TextMeshProUGUI scoreTextMesh = scoreText.GetComponent<TextMeshProUGUI>();
        if (scoreTextMesh != null)
        {
            scoreTextMesh.text = "0";
        }

        // Make the existing ball disappear and create a new one with a random color.
        Destroy(ball);
        NewBall();
    }

    public void RotatePlayerLeft() {
        if (IsMovementDisabled()) return;
        playerCameraRig.transform.Rotate(0, -1 * rotateSpeed, 0);
    }

    public void RotatePlayerRight() {
        if (IsMovementDisabled()) return;
        playerCameraRig.transform.Rotate(0, rotateSpeed, 0);
    }

    public void ThrowBall() {
        // We need to descend and get the InteractableGroupView to determine grabbed state.
        Transform interactable = ball.transform.Find("GamePiece Interactable");
        if (interactable != null) {
            Transform handGrabInteraction = interactable.transform.Find("ISDK_DistanceHandGrabInteraction");
            if (handGrabInteraction != null) {
                InteractableGroupView groupView = handGrabInteraction.GetComponent<InteractableGroupView>();
                // If the ball is currently in possession and user hits trigger then launch it.
                if(groupView.SelectingInteractorsCount > 0) {
                    // Reset grabbable component to release the grab.
                    var grabbable = handGrabInteraction.GetComponent<Grabbable>();
                    grabbable.enabled = false;
                    grabbable.enabled = true;
                    
                    // Apply force to the ball in the forward direction of the hand.
                    Rigidbody ballRigidbody = interactable.GetComponent<Rigidbody>();
                    ballRigidbody.AddForce(handGrabInteraction.forward * throwingForce, ForceMode.Impulse);
                }
            }
        }
    }

    public void ToggleMenu() {
        if (menu.activeSelf) {
            menu.SetActive(false);
        } else {
            // From the camera rig find eye anchor to calculate move relative to the player's forward direction.
            Transform trackingSpace = playerCameraRig.transform.Find("TrackingSpace");
            Transform centerEyeAnchor = trackingSpace.transform.Find("CenterEyeAnchor");

            if (centerEyeAnchor != null) {
                // Set the menu in front of the player.
                Vector3 menuPosition = centerEyeAnchor.position + centerEyeAnchor.forward * menuDistance;
                menu.transform.position = new Vector3(menuPosition.x, centerEyeAnchor.position.y, menuPosition.z);
                // Rotate the menu to face the player.
                menu.transform.LookAt(centerEyeAnchor);
                // Correct mirroring.
                menu.transform.rotation = Quaternion.Euler(0, menu.transform.rotation.eulerAngles.y + 180, 0);
            }
            menu.SetActive(true);
        }
    }
}
