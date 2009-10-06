using UnityEngine;
using System.Collections;

public class FishFleeBehaviour : FishVelocityDrivenSeekingBehaviour {

    public override Vector3 Direction(){
        return -base.Direction();
    }
}
