using UnityEngine;
using System.Collections;

namespace MurderMystery {

    public class PlayerAnimation : MonoBehaviour {

        private Animator myAnimator;

        // Use this for initialization
        void Start() {
            myAnimator = GetComponent<Animator>();
        }

        //faceXX: Changes our "direction_x" and "direction_y" animator variables to reflect the direction we wish to face.
        public void faceLeft() {
            myAnimator.SetFloat("direction_x", -1);
            myAnimator.SetFloat("direction_y", 0);
        }
        public void faceUp() {
            myAnimator.SetFloat("direction_x", 0);
            myAnimator.SetFloat("direction_y", 1);
        }
        public void faceRight() {
            myAnimator.SetFloat("direction_x", 1);
            myAnimator.SetFloat("direction_y", 0);
        }
        public void faceDown() {
            myAnimator.SetFloat("direction_x", 0);
            myAnimator.SetFloat("direction_y", -1);
        }

        //move: Changes our "isMoving" animator variable to reflect our desire to move.
        public void move() {
            myAnimator.SetBool("isMoving", true);
        }
        //stopMove: Changes our "isMoving" animator variable to reflect our desire to stop moving.
        public void stopMove() {
            myAnimator.SetBool("isMoving", false);
        }

    }

}