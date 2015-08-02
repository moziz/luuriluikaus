using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour
{

    GameObject numberChild;
    int numbersToReceive = 5;

    void Start()
    {
        numberChild = transform.GetChild(0).gameObject;
        PhoneController.instance.SubscribeOnRotaryEnd(UnrollFinished);
    }
    
    void Update()
    {
        if(numbersToReceive >= 0)
        {
            numberChild.SetActive(Time.time % 2 < 1);
        }
    }

    void UnrollFinished()
    {
        if(--numbersToReceive < 0)
        {
            GameObject.Find("Player").GetComponent<PlayerCharacter>().StartGame();
            gameObject.SetActive(false);
        }
    }
}
