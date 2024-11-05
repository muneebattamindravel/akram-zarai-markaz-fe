using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using TMPro;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using UnityEngine.UI;

public class Screen_TransfersList : OSA<BaseParamsWithPrefab, TransfersListViewHolder>
{
    public GameObject contentRoot;
    public List<Transfer> transfers;
    public List<ColumnHeader> columnHeaders;
    public MRDateFilterPicker dateFilterPicker;
    public TMP_Text text_totalTransfers;

    public SimpleDataHelper<Transfer> Data { get; private set; }
    protected override void Start()
    {
        Data = new SimpleDataHelper<Transfer>(this);
        base.Start();
    }

    protected override TransfersListViewHolder CreateViewsHolder(int itemIndex)
    {
        var instance = new TransfersListViewHolder();
        instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        return instance;
    }

    protected override void UpdateViewsHolder(TransfersListViewHolder newOrRecycled)
    {
        Transfer transfer = Data[newOrRecycled.ItemIndex];

        newOrRecycled.id.GetComponent<TMP_Text>().text = transfer.id.ToString();
        newOrRecycled.bookNumber.GetComponent<TMP_Text>().text = transfer.bookNumber.ToString();
        newOrRecycled.billNumber.GetComponent<TMP_Text>().text = transfer.billNumber.ToString();

        newOrRecycled.fromPartnerBg.gameObject.SetActive(false);
        newOrRecycled.toPartnerBg.gameObject.SetActive(false);

        if (transfer.fromAccount.type == AccountType.Partner.ToString())
            newOrRecycled.fromPartnerBg.gameObject.SetActive(true);
        if (transfer.toAccount.type == AccountType.Partner.ToString())
            newOrRecycled.fromPartnerBg.gameObject.SetActive(true);

        newOrRecycled.date.GetComponent<TMP_Text>().text = transfer.date.ToString(Constants.DateDisplayFormat);
        newOrRecycled.amount.GetComponent<TMP_Text>().text = transfer.amount.ToCommaSeparatedNumbers() + Constants.Currency;
        newOrRecycled.fromAccountName.GetComponent<TMP_Text>().text = transfer.fromAccount.name;
        newOrRecycled.toAccountName.GetComponent<TMP_Text>().text = transfer.toAccount.name;

        newOrRecycled.edit.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.edit.GetComponent<MRButton>().onClicked.AddListener(() => {
            EditTransfer(transfer.id);
        });

        newOrRecycled.delete.GetComponent<MRButton>().onClicked.RemoveAllListeners();
        newOrRecycled.delete.GetComponent<MRButton>().onClicked.AddListener(() => {
            DeleteTransfer(transfer.id);
        });
    }

    private void OnEnable()
    {
        GetTransfers();

        TransfersManager.onTransferAdded += GetTransfers;
        TransfersManager.onTransferDeleted += GetTransfers;

        dateFilterPicker.gameObject.SetActive(true);
        dateFilterPicker.onDateSelected += GetTransfers;

        InitializeColumnsHeaders();
    }

    private void OnDisable()
    {
        TransfersManager.onTransferAdded -= GetTransfers;
        TransfersManager.onTransferDeleted -= GetTransfers;

        dateFilterPicker.onDateSelected -= GetTransfers;

        transfers = null;
        GC.Collect();
    }

    public void InitializeColumnsHeaders()
    {
        foreach (ColumnHeader header in columnHeaders)
        {
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.RemoveAllListeners();
            header.gameObject.transform.Find("Button_Heading").GetComponent<MRButton>().onClicked.AddListener(() => {
                foreach (ColumnHeader hdr in columnHeaders)
                    if (hdr != header)
                        hdr.ResetState();

                ColumnState nextState = header.SetNextState();
                FieldInfo fieldInfo = typeof(Transfer).GetField(header.dataField);
                if (nextState == ColumnState.ASCENDING)
                    transfers = transfers.OrderBy(p => fieldInfo.GetValue(p)).ToList();
                else if (nextState == ColumnState.DESCENDING)
                    transfers = transfers.OrderByDescending(p => fieldInfo.GetValue(p)).ToList();
                else
                    transfers = transfers.OrderBy(p => p.id).ToList();

                PopulateData();
            });

            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.RemoveAllListeners();
            header.gameObject.transform.Find("InputField_Filter").GetComponent<TMP_InputField>().onValueChanged.AddListener((endValue) => {
                foreach (Transfer item in transfers) item.IsEnabledOnGrid = true;
                FieldInfo fieldInfo = typeof(Transfer).GetField(header.dataField);
                foreach (Transfer filtered in transfers.FindAll(p => !fieldInfo.GetValue(p).ToString().ToLower().Contains(header.GetFilterValue().ToLower())))
                    filtered.IsEnabledOnGrid = false;

                PopulateData();
            });
        }
    }

    void GetTransfers()
    {
        Preloader.Instance.ShowWindowed();
        TransfersManager.Instance.GetTransfers(dateFilterPicker.GetDateRange(), (response) => {
            transfers = response.data;
            columnHeaders[1].SetState(ColumnState.DESCENDING);
            PopulateData();
        }, null);
    }

    void PopulateData()
    {
        Preloader.Instance.ShowWindowed();

        float totalTransfers = 0f;
        foreach (Transfer t in transfers)
            totalTransfers += t.amount;
        text_totalTransfers.text = totalTransfers.ToCommaSeparatedNumbers();

        if (this.Data.Count > 0)
            this.Data.RemoveItems(0, this.Data.Count);
        this.Data.InsertItems(0, transfers.FindAll(p => p.IsEnabledOnGrid));

        Preloader.Instance.HideWindowed();
    }

    void DeleteTransfer(int transferId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Confirmation);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_ConfirmationDialogue>().ShowView(
            Constants.DeleteConfirmation + Constants.Transfer,
            Constants.DELETE, Constants.CANCEL,
            () => {
                //Confirmed
                Preloader.Instance.ShowFull();
                TransfersManager.Instance.DeleteTransfer(transferId, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Success, Constants.TransferDeleted);
                }, (response) => {
                    Preloader.Instance.HideFull();
                    GUIManager.Instance.ShowToast(Constants.Error, response.message.message, false);
                });
            },
            () => {
                //Cancelled
            }
        );
    }

    void EditTransfer(int transferId)
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Transfers_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Transfers_View_Add>().ShowView(transferId);
    }

    public void Button_AddClicked()
    {
        GUIManager.Instance.OpenScreenExplicitly(MRScreenName.Transfers_View_Add);
        GUIManager.Instance.CURRENTPANEL.GetComponent<Screen_Transfers_View_Add>().ShowView();
    }

    public void Button_ReloadClicked()
    {
        GetTransfers();
    }
}

public class TransfersListViewHolder : BaseItemViewsHolder
{
    public TMP_Text id, date, fromAccountName, toAccountName, bookNumber, billNumber, amount;
    public Image fromPartnerBg, toPartnerBg;
    public MRButton edit, delete;

    public override void CollectViews()
    {
        base.CollectViews();

        root.GetComponentAtPath("id", out id);
        root.GetComponentAtPath("date", out date);
        root.GetComponentAtPath("fromPartnerBg", out fromPartnerBg);
        root.GetComponentAtPath("toPartnerBg", out toPartnerBg);
        root.GetComponentAtPath("fromAccountName", out fromAccountName);
        root.GetComponentAtPath("toAccountName", out toAccountName);
        root.GetComponentAtPath("bookNumber", out bookNumber);
        root.GetComponentAtPath("billNumber", out billNumber);
        root.GetComponentAtPath("amount", out amount);
        root.GetComponentAtPath("Button_Edit", out edit);
        root.GetComponentAtPath("Button_Delete", out delete);
    }
}