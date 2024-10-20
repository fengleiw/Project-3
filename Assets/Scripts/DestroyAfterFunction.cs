using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterFunction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
    }

   
}
