using UnityEngine;
using System.Collections;

public class PreyFishAI : FishAI, IBitable {
    public override void OnHit(Spear spear){
        base.OnHit(spear);
        SendMessage("OnDestroyGameObject", null, SendMessageOptions.DontRequireReceiver);
		Die();
     }
     
     public void OnBite(){
         DestroyGameObject();
     }
}