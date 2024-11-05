using UnityEngine;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using UnityEngine.UI;

public class Screen_Contacts : OSA<BaseParamsWithPrefab, MRContactsListViewHolder>
{
	public SimpleDataHelper<MRContact> Data { get; private set; }
	protected override void Start()
	{
		Data = new SimpleDataHelper<MRContact>(this);
		base.Start();
	}

	public void Button_LoadClicked()
    {
		PopulateDataList();
	}

	public void Button_RemoveClicked()
    {
		this.Data.RemoveItems(0, this.Data.Count);
    }

	void PopulateDataList()
    {
		this.Data.InsertItems(0, MRContactsManager.Instance.contacts);
	}

	protected override MRContactsListViewHolder CreateViewsHolder(int itemIndex)
	{
		var instance = new MRContactsListViewHolder();
		instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
		return instance;
	}

	protected override void UpdateViewsHolder(MRContactsListViewHolder newOrRecycled)
	{
		MRContact model = Data[newOrRecycled.ItemIndex];
		newOrRecycled.text_number.text = "# " + newOrRecycled.itemIndexInView.ToString();
		newOrRecycled.text_name.text = model.name;
		newOrRecycled.text_phone.text = model.phone;

		newOrRecycled.button_item.onClick.RemoveAllListeners();
		newOrRecycled.button_item.onClick.AddListener(() => {
			Debug.Log("Clicked = " + model.name + " " + model.phone);
		});
	}
}

public class MRContactsListViewHolder : BaseItemViewsHolder
{
	public Text text_number, text_name, text_phone;
	public Button button_item;

	public override void CollectViews()
	{
		base.CollectViews();

		root.GetComponentAtPath("Text_Number", out text_number);
		root.GetComponentAtPath("Text_Name", out text_name);
		root.GetComponentAtPath("Text_Phone", out text_phone);
		root.GetComponentAtPath("Button_Item", out button_item);
	}
}
