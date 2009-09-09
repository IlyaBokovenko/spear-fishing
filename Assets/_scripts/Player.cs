using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform gun;
    public GameObject bullet ;
    public float firingPower = 2000;

    public float speed  = 4; 

    // void Start(){
    //     Fire();
    // }


    void Awake() {
        Screen.SetResolution(480, 320, true);
    }

    void FixedUpdate(){
       rigidbody.velocity = transform.forward * speed;
    }

    void OnApplicationQuit(){
        Profiler.PrintResults();
    }

    void Update () {

        Vector3 rotation = transform.localEulerAngles;
        rotation += 2 * new Vector3(-Input.GetAxis ("Mouse Y"), Input.GetAxis ("Mouse X"), 0);
        transform.localEulerAngles = rotation;

        fireIfNeeded();
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void fireIfNeeded(){
       if(Input.GetKeyDown(KeyCode.Space)) {
           Fire();
       }
    }

    void Fire(){
        GameObject newBullet  = (GameObject)Instantiate(bullet, gun.position, transform.rotation);
        Physics.IgnoreCollision(collider, newBullet.collider);
        newBullet.transform.localScale = bullet.transform.lossyScale;
        newBullet.rigidbody.velocity = rigidbody.velocity;
        Spear spear  = (Spear)newBullet.GetComponent(typeof(Spear));
        spear.FireWithForce(firingPower);        
    }
}