using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    public Animator myDoor = null;
    public bool openTrigger = false;
    public bool closeTrigger = false;
    Animator animator;
    private void Start() {
        animator = GetComponentInParent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Puertaaaa");
            if (openTrigger)
            {
                animator.Play("Door_Open", 0, 0.0f);
                gameObject.SetActive(false);
            }
            else if (closeTrigger)
            {
                //animator.Play("Door_Close", 0, 0.0f);
                gameObject.SetActive(false);
            }
        }
    }
}