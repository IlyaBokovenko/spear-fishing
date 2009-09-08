using UnityEngine;
using System.Collections;

public class FishAnimation : GenericScript
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start(){
        RememberAnimation();
    }

    void Update () {
        ResetAnimation();
    }

//////////////////////////////////////////////////

    private void ResetAnimation(){
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;            
    }

    private void RememberAnimation(){
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;    
    }    
}

