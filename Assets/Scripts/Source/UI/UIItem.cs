using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour {

    ProgramData myData;
    Button InfoButton;
    List<string> extraData = new List<string>();

    public static Image Spawn(Image itemprefab, ProgramData program, Transform parent){
		Image go = Instantiate(itemprefab);
        go.GetComponentInChildren<Text>().text = program.Titles[program.Language];
        go.transform.SetParent(parent);
		go.enabled = true;
        return go.GetComponent<Image>();
    }

	void Start()
	{
        InfoButton = GetComponent<Button>();
		InfoButton.onClick.AddListener(() =>
		{
            UIManager.Instance.ExtraData(extraData);
		});

	}

    public void SetData(ProgramData data){
        myData = data; 
        extraData.Add(myData.Titles[myData.Language]);
        extraData.Add(myData.programCollection);
        extraData.Add(myData.mediaType);
        extraData.Add(myData.Language);
        extraData.Add(myData.idProgram);
        extraData.Add(myData.durationProgram);
    }

}
