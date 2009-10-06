using UnityEngine;
using System.Collections;

public class FishFleeBehaviour : FishOrientationDrivenSeekingBehaviour {

    public override Vector3 Direction(){
        return -base.Direction();
    }
}
