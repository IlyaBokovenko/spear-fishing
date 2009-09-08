using UnityEngine;
using System.Collections;

public class PreyFishAI : FishAI {
    public override void OnHit(Spear spear){
         base.OnHit(spear);
         Die();
     }
}