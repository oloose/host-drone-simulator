using UnityEngine;

/// <summary>
/// Applies a force to a gameobject that collided with this gameobject. 
/// The force direction is the vector between the collsion point and the position (pivot point) of the collided with gameobject. 
/// </summary>
public class WindturbineColliderForce : MonoBehaviour {

    private float hitThrust = 40f;

    private void OnCollisionEnter(Collision collision) {
        //Get gameobject we collided with
        GameObject collidedWithGameobject = collision.gameObject;

        //check if collided with object is child of player gameobject (DroneController is a singleton and direct component of the Player gameobject)
        if(collidedWithGameobject.transform.IsChildOf(DroneController.Instance.gameObject.transform)) {

            //calculate force direction
            Vector3 collisionPoint = collision.contacts[0].point;
            Vector3 collidedGameobjectPosition = collidedWithGameobject.transform.position;       
            Vector3 forceVector = collidedGameobjectPosition - collisionPoint;

            //apply force as single impulse in forceVector direction
            collidedWithGameobject.GetComponent<Rigidbody>().AddForce(forceVector * hitThrust, ForceMode.Impulse);
        }
    }
}
