using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    Button SearchButton;
   
    [SerializeField]
    InputField TitleSearch;

    [SerializeField]
    GameObject content;

    [SerializeField]
	Scrollbar Scroll;

	[SerializeField]
	Image programItem;

    [SerializeField]
    GameObject prefabData;

	[SerializeField]
    List<Text> data = new List<Text>();


    List<ProgramData> programsList = new List<ProgramData>();
    List<Image> titleItem = new List<Image>();

    void Start(){
        SearchButton.onClick.AddListener(() =>{
           if(TitleSearch.text != ""){
                ServerManager.Instance.DataRequest(TitleSearch.text);
				SearchButton.interactable = false;
			}
        });

        Scroll.onValueChanged.AddListener((el) => {
            UpdateList();
           
        });
    }

	public void OmLoaded(MainData data)
	{
		string title;

		ClearView();
		foreach (ProgramData program in data.programs)
		{
			if (program.Titles.TryGetValue(program.Language, out title))
			{
				programsList.Add(program);
			}
		}
		UpdateList();
		TitleSearch.text = "";
		SearchButton.interactable = true;
	}

    void UpdateList(){
		if (Scroll.value != 0)
			return;
		int counter = titleItem.Count;
		for (int i = counter; i < programsList.Count; i++){
			if (i == counter + 10)
				break;
            Image program = UIItem.Spawn(programItem, programsList[i], content.transform);
			titleItem.Add(program);
            program.GetComponent<UIItem>().SetData(programsList[i]);
		}
		int titlesNext = titleItem.Count;
		if (titlesNext > 10)
			Scroll.value = 1 / (titlesNext / 10);

	}
	private void ClearView(){
        if(titleItem.Count > 0){
			programsList.Clear();
           
			foreach (Image image in titleItem)
			{
                if(image.gameObject != null)
                    Destroy(image.gameObject);
			}
			titleItem.Clear();

        }


	}

    public void ExtraData(List<string> info){
        if (!prefabData.activeInHierarchy){
            prefabData.SetActive(true);
        }
        for (int i = 0; i < data.Count;i++){
            data[i].text = info[i];
        }
    }

}
