﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorBreachStateBehavior : StateMachineBehaviour {

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("Finished", true);
        //animator.enabled = false;
        animator.GetComponent<ReactorBreachBehavior>().DestroyMe();
    }
}