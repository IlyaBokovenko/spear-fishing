using UnityEngine;
using System.Collections;

public class FishFleeBehaviour : FishSeekingBehaviour {

    protected override Vector3 direction(){
        return -base.direction();
    }
}
