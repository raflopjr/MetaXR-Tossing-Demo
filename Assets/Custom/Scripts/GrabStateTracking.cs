using UnityEngine;
using Oculus.Interaction;

public class GrabStateTracking : MonoBehaviour {
   private DistanceGrabInteractor _interactable;
   private IRelativeToRef _relativeToRef;
   
   void Start()
   {
      _interactable = gameObject.GetComponent<DistanceGrabInteractor>();
      if (_interactable != null)
      {
         _relativeToRef = _interactable.DistanceInteractable;
      }
   }

   void Update(){
        Debug.Log("Connected to hand " + _relativeToRef);
   }
}