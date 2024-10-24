using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    private void Start()
    {
        if(transitionTo == GameManager.Instance.transitionedFromScene)
        {
            PlayerController.instance.transform.position = startPoint.position;
            StartCoroutine(PlayerController.instance.WalkIntoNewScene(exitDirection, exitTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            PlayerController.instance.pState.cutScene = true;
            SceneManager.LoadScene(transitionTo);
        }
    }

}
