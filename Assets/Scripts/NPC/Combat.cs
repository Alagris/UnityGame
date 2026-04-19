using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace Inv
{
    public class Combat : MonoBehaviour
    {
        /**Every attack animation consists of 3 stages:
        1. animation plays and player cannot attack again. Clicking attack button is ignored.
        2. animation plays and player cannot attack again but if at this stage player clicks the attack button, the game registers that player wants to attack again.
        3. animation plays but player can already chose to execute the next attack, which would interrupt the current attack animation. If game registered any attack clicks in previous stage, then the next attack is launched immediately.
        4. end of animation. If this end is reached but player hasn't clicked the attack button teh whole time, then the idle animation starts playing.

       This is what variables are set at each stage:
       1. bufferAttackRequests=false,  canAttack=false, canMove=false, wantsToAttack=false
       2. bufferAttackRequests=true,  canAttack=false, canMove=false, wantsToAttack=maybe
       3. bufferAttackRequests=false,  canAttack=true, canMove=false, stage ends if wantsToAttack
       (between 3 and 4.) next attack plays if player clicks attack button
       4. bufferAttackRequests=false,  canAttack=true, canMove=true, wantsToAttack=false

       It is possible that the stage 2 is never entered. In which case no attack buffering occurs.
       */
        protected bool canAttack = true;
        protected bool bufferAttackRequests = true;
        protected bool wantsToAttack = false;
        [SerializeField]
        AnyCharacterController characterController;
        

        private void Start()
        {
            if(characterController == null)
            {
                characterController = GetComponent<AnyCharacterController>();
            }
        }

        static string b(bool b) => b ? "T" : "F";
        void DebugLog(string msg)
        {

            Debug.Log(

                "[canMove=" + b(characterController.canMove) +
                " canAttack=" + b(canAttack) +
                " bufferAttackRequests=" + b(bufferAttackRequests) +
                " wantsToAttack=" + b(wantsToAttack)
                + "]" + msg
            );
        }

        public void ForceAttack()
        {
            characterController.GetCharacterInstance().TriggerAttack();
            bufferAttackRequests = false;
            canAttack = false;
            characterController.DisableMovement(AnyCharacterController.DISABLE_MOVMEMENT_BY_COMBAT);
            wantsToAttack = false;
            DebugLog("Attack started");
        }

        public void Attack()
        {
            if (characterController.inventory.hasEquippedInHand())
            {
                if (canAttack) // if in stage 3
                {   
                    characterController.inventory.EquippedInHand.OnAttack(characterController);
                }
                else if (bufferAttackRequests) // if in stage 2
                {
                    wantsToAttack = true;
                    DebugLog("Wants to attack");
                }
            }
        }
       
        public void OnAttackCanListen() // enters stage 2
        {

            bufferAttackRequests = true;
            DebugLog("Attack buffer listening");
        }
        public void OnAttackReady() // enters stage 3
        {
            if (wantsToAttack)
            { // if attack was buffered, execute it


                characterController.GetCharacterInstance().TriggerAttack();
                wantsToAttack = false;
                // we leave bufferAttackRequests==true because the OnAttackEnd gets triggered even when the animation is interrupted.
                // We need some way to decide whetehr the animation ended normally or was interrupted. bufferAttackRequests is gonna be such flag
                DebugLog("Ready to attack, executing next");
            }
            else
            {

                canAttack = true; // enters stage 3
                bufferAttackRequests = false; // exists stage 2
                DebugLog("Ready to attack");
            }


        }
        public void OnSheathingStart(string debugString)
        {
            DebugLog("Sheathing animation started (" + debugString + ")");
        }
        public void OnAttackStart(string debugString)
        {
            DebugLog("Attack animation started (" + debugString + ")");
        }
        public void OnAttackEnd(bool beginSheathing, string debugString) // enters stage 4
        {

            canAttack = true;
            wantsToAttack = false;
            if (bufferAttackRequests) // if true, that means the animation was interrupted
            {
                bufferAttackRequests = false;
                DebugLog("Attack ended early (" + debugString + ")");
            }
            else
            {
                characterController.SetMovementEnabled(AnyCharacterController.DISABLE_MOVMEMENT_BY_COMBAT, !beginSheathing);
                if (beginSheathing)
                {
                    DebugLog("Sheathing weapon (" + debugString + ")");
                }
                else
                {
                    DebugLog("Attack ended (" + debugString + ")");
                }

            }

        }

        public void OnSheathingEnd(string debugString)
        {
            characterController.EnableMovement(AnyCharacterController.DISABLE_MOVMEMENT_BY_COMBAT);
            DebugLog("Sheathing ended (" + debugString + ")");

        }

    }
}
