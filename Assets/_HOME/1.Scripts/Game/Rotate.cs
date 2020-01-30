﻿using UnityEngine;

namespace HOME.Game {

    public class Rotate : MonoBehaviour {

        public Vector3 Rotation = Vector3.zero;

        void Update() {

            transform.Rotate(Rotation * Time.deltaTime);
        }
    }
}
