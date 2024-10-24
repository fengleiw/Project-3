using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    PlayerController player;
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParents;
    public GameObject heartContainerPrefab;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        player = PlayerController.instance;
        heartContainers = new GameObject [PlayerController.instance.maxHealth];
        heartFills = new Image[PlayerController.instance.maxHealth];

        PlayerController.instance.onHealthChangeCallback += UpdateHeartsHUD;

        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }

    
    void Update()
    {
        
    }
    
    void SetHeartContainers()
    {
        for(int i = 0; i < heartContainers.Length; i++) 
        {
            if(i < PlayerController.instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            } else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHeart()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < PlayerController.instance.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }
    void InstantiateHeartContainers()
    {
        for (int i = 0; i <PlayerController.instance.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParents, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }

    }

    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHeart();
    }
}
