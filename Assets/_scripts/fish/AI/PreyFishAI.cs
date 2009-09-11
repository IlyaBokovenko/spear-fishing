using UnityEngine;
using System.Collections;

public class PreyFishAI : FishAI, IBitable {
    public override void OnHit(Spear spear){
         base.OnHit(spear);
         Die();
     }
     
     public void OnBite(FishHuntingTargetBehaviour hunter){
         print("bitten");
         Die();
     }
}