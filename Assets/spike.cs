//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using TMPro.EditorUtilities;
//using UnityEngine;


//public class spike : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    private void OnTriggerEnter2D(Collider2D _collision)
//    {
//        if (_collision.composite.CompareTag("Player"))
//        {
//            StartCoroutine(RespawnPoint());
//        }
//    }

//    IEnumerator RespawnPoint()
//    {
//        PlayerController.instance.pState.cutScene = true;
//        PlayerController.instance.pState.invincible = true;
//        PlayerController.instance.rb.velocity = Vector2.zero;
//        Time.timeScale = 0;
//        StartCoroutine(UIManager.instance.sceneFader.Fade(SceneFader.FadeDirection));
//        PlayerController.instance.TakeDamage(1);
//        yield return new WaitForSeconds(1);
//        PlayerController.instance.transform.position = GameManager.Instance.platformingReSpawnPoint;
//        StartCoroutine(UIManager.instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
//        yield return new WaitForSeconds(TMP_UIStyleManager.instance.sceneFader.fadeTime);
//        PlayerController.instance.pState.cutScene = false;
//        PlayerController.instance.pState.invincible = false;
//        Time.timeScale = 1;
        
//    }
//}
