using UnityEngine;
using System.Collections;

public class PredatorFishAI : FishAI {
    public override void OnHit(Spear spear){
        Transform myAnimatedBody = transform.Find("animatable");
        spear.DecorativeStickOn(myAnimatedBody.gameObject);
    }
}