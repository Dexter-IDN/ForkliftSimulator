using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class AreaAQuestManager : MonoBehaviour
{
    private int count;
    
    public int limit;
    public string area;
    public string itemTag;
    public Text areaText;
    public Text questText;
    // Start is called before the first frame update
    void Start()
    {
        UpdateAreaName();
        UpdateQuestText();  
        UpdateLimitInGameManager(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == itemTag) {
            if(count < limit) {
                count++;
                UpdateQuestText();
                UpdateAreaName();
                UpdateCountInGameManager();
            }
        }
    }

    void UpdateCountInGameManager() {
        GameManager.count += count;
    }

    void UpdateLimitInGameManager() {
        GameManager.limit += limit;
    }

    void UpdateAreaName() {
        areaText.text = "Area " + area + " (" + count + "/" + limit + ")";
    }

    void UpdateQuestText() {
        questText.text = "Pindahkan " + itemTag + " ke Area " + area + " (" + count + "/" + limit + ")";
    }
}
