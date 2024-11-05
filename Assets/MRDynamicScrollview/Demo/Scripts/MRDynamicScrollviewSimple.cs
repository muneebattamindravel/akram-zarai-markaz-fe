using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;
using UnityEngine.Events;

public class MRDynamicScrollviewSimple : OSA<BaseParamsWithPrefab, BaseItemViewsHolder>
{
    public UnityEvent OnItemsUpdated;
	//public List<T> dataList;

	protected override void Start()
	{
		//dataList = new List<T>();
		base.Start();
	}

	protected override BaseItemViewsHolder CreateViewsHolder(int itemIndex)
	{
		var instance = new BaseItemViewsHolder();
		instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
		return instance;
	}

	protected override void UpdateViewsHolder(BaseItemViewsHolder newOrRecycled)
	{
		//T model = dataList[newOrRecycled.ItemIndex];

		//newOrRecycled.backgroundImage.color = model.color;
		//newOrRecycled.UpdateTitleByItemIndex(model);
		//newOrRecycled.icon1Image.texture = _Params.availableIcons[model.icon1Index];
		//newOrRecycled.icon2Image.texture = _Params.availableIcons[model.icon2Index];
	}

	public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
	{
		base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);

		if (OnItemsUpdated != null)
			OnItemsUpdated.Invoke();
	}
}

	