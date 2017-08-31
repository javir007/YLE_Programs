using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour {

    public static Image Spawn(Image itemprefab, string Title, Transform parent){
		Image go = Instantiate(itemprefab);
        go.GetComponentInChildren<Text>().text = Title;
        go.transform.SetParent(parent);
		go.enabled = true;

        return go.GetComponent<Image>();
    }
}
