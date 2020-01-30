using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    public class Bullet : Projectile {

        public override void Update() {
            if (target == null) { // no target just return
                return;
            }
            base.Update();
            Vector3 direction = target.position - transform.position; // get dir
            float distanceThisFrame = speed * Time.deltaTime;// get distance moved this frame
            if (direction.magnitude <= distanceThisFrame) { // if the length of the magnitude is less then distanceThis frame? then we already hit the target!
                HitTarget();
                return;
            }
            transform.Translate(direction.normalized * distanceThisFrame, Space.World); // travel!
        }
    }
}
