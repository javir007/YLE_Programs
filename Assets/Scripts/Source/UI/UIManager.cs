using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IEventListener
{
    MainData data;

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

    List<string> titles = new List<string>();
    List<Image> titleItem = new List<Image>();

    public void OnEvent(string eventName, object content){
        if (eventName == ServerBroadcaster.EventName.OnLoaded){
            string title;
            ClearView();
            MainData data = (MainData)content;
            foreach(ProgramData program in data.programs){
                if(program.Titles.TryGetValue(program.Language,out title)){
                    titles.Add(title);
                }
            }
            UpdateList();
		    TitleSearch.text = "";
            SearchButton.interactable = true;
		}
    }

    void Start(){
        SearchButton.onClick.AddListener(() =>
        {
            if(TitleSearch.text != ""){
                AppEvents.Instance.Server.Fire(ServerBroadcaster.EventName.OnRequest, TitleSearch.text);
				SearchButton.interactable = false;
			}
          
        });

        Scroll.onValueChanged.AddListener((el) => 
        {
            UpdateList();
           
        });
    }
	
    void UpdateList(){
		if (Scroll.value != 0)
			return;

		int counter = titleItem.Count;
		for (int i = counter; i < titles.Count; i++)
		{
			if (i == counter + 10)
				break;
            Image program = UIItem.Spawn(programItem, titles[i], content.transform);
			titleItem.Add(program);
		}
		int titlesNext = titleItem.Count;
		if (titlesNext > 10)
			Scroll.value = 1 / (titlesNext / 10);

	}
	private void ClearView()
	{
		titles.Clear();
        foreach(Image image in titleItem){
            Destroy(image.gameObject);
        }
	}

}
