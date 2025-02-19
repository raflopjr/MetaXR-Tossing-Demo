using UnityEngine;
using Oculus.Interaction;

public class GamePiecePokeLauncher : MonoBehaviour
{
    public Rigidbody gamePieceRigidbody;
    public float throwForce = 20f;

    public void ThrowGamePiece() {
        Debug.Log("Throwing game piece!");
        GameObject parentGameObject = transform.parent.gameObject;
        Grabbable siblingGrabbable = parentGameObject.transform.parent.GetComponentInChildren<Grabbable>();
        siblingGrabbable.enabled = false;
        siblingGrabbable.enabled = true;

        // Launch the game piece forward from where its aimed.
        Transform parentTransform = parentGameObject.transform.parent;
        gamePieceRigidbody.AddForce(parentTransform.forward * throwForce, ForceMode.Impulse);
    }
}
