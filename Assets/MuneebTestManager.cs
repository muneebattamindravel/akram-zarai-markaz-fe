using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int score;
    public string name;

    public Item(int _score, string _name)
    {
        score = _score;
        name = _name;
    }
}

public class MuneebTestManager : MonoBehaviour
{
    public List<Transaction> meezanTransactions, mindravelTransactions, tplTransactions,
        faysalTransactions,muneebscbTransactions,muneebAskariTransactions,saadTransactions,ahmadTransactions;


    

    public void Button_ReadFilesClicked()
    {

        string csvDirectoryPath = "Assets/TestMuneeb/";

        string fileName = "meezan.csv";
        string filePath = Path.Combine(csvDirectoryPath, fileName);
        meezanTransactions = ReadCsvFile(filePath);

        fileName = "faysal.csv";
        filePath = Path.Combine(csvDirectoryPath, fileName);
        faysalTransactions = ReadCsvFile(filePath);

        fileName = "mindravel.csv";
        filePath = Path.Combine(csvDirectoryPath, fileName);
        mindravelTransactions = ReadCsvFile(filePath);

        fileName = "tpl.csv";
        filePath = Path.Combine(csvDirectoryPath, fileName);
        tplTransactions = ReadCsvFile(filePath);

        fileName = "muneebscb.csv";
        filePath = Path.Combine(csvDirectoryPath, fileName);
        muneebscbTransactions = ReadCsvFile(filePath);

        fileName = "muneebaskari.csv";
        filePath = Path.Combine(csvDirectoryPath, fileName);
        muneebAskariTransactions = ReadCsvFile(filePath);

        fileName = "saad.csv";
        filePath = Path.Combine(csvDirectoryPath, fileName);
        saadTransactions = ReadCsvFile(filePath);

        fileName = "ahmad.csv";
        filePath = Path.Combine(csvDirectoryPath, fileName);
        ahmadTransactions = ReadCsvFile(filePath);

        string seedLog = "************* M E E Z A N ***************";
        ProcessTransactions(meezanTransactions, ref seedLog);
        WriteFile(filePath + "meezan-log.txt",  seedLog);

        seedLog = "************* M U N E E B   S C B ***************";
        ProcessTransactions(muneebscbTransactions, ref seedLog);
        WriteFile(filePath + "muneebscb-log.txt",  seedLog);

        seedLog = "************* M U N E E B   A S K A R I ***************";
        ProcessTransactions(muneebAskariTransactions, ref seedLog);
        WriteFile(filePath + "muneebaskari-log.txt",  seedLog);

        seedLog = "************* S A A D ***************";
        ProcessTransactions(saadTransactions, ref seedLog);
        WriteFile(filePath + "saad-log.txt",  seedLog);

        seedLog = "************* A H M A D ***************";
        ProcessTransactions(ahmadTransactions, ref seedLog);
        WriteFile(filePath + "ahmad-log.txt",  seedLog);

        seedLog = "************* F A Y S A L ***************";
        ProcessTransactions(faysalTransactions, ref seedLog);
        WriteFile(filePath + "faysal-log.txt",  seedLog);

        Debug.Log("ALL DONE");
    }

    void WriteFile(string filePath, string content)
    {

        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (StreamWriter writer = new StreamWriter(fs))
            writer.Write(content);
    }

    void ProcessTransactions(List<Transaction> transactions, ref string seedLog)
    {
        int i = 1;
        foreach (var transaction in transactions)
        {
            if (

                (
                mindravelTransactions.Find(p => p.Amount == transaction.Amount) == null
                &&
                tplTransactions.Find(p => p.Amount == transaction.Amount) == null
                )

                )
            {
                seedLog += $"\n{i++} Date: {transaction.Date.ToString("dd/MM/yyyy")}, Payee {transaction.Payee} , Amount: {transaction.Amount}";
            }
        }

    }

    private List<Transaction> ReadCsvFile(string filePath)
    {
        List<Transaction> transactions = new List<Transaction>();

        using (var reader = new StreamReader(filePath))
        {
            // Skip the header line
            string headerLine = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = ParseCsvLine(line);

                if (values.Length == 13) // Ensure there are exactly 13 columns
                {
                    Transaction transaction = new Transaction
                    {
                        Type = values[0],
                        Status = values[1],
                        Date = DateTime.ParseExact(values[2], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Payee = values[3],
                        CategoryOrAccount = values[4],
                        Amount = CleanAmount(values[5]),
                        Note = values[6],
                        Memo = values[7],
                        Number = values[8],
                        Security = values[9],
                        Shares = values[10],
                        PricePerShare = values[11],
                        Commission = values[12]
                    };

                    transactions.Add(transaction);
                }
                else
                {
                    Debug.LogWarning($"Skipping invalid row in {filePath}: {line}");
                }
            }
        }

        return transactions;
    }

    private int CleanAmount(string amount)
    {
        // Use regular expression to remove currency symbols, commas, and whitespace
        string cleanedAmount = Regex.Replace(amount, @"[\s,]", "");
        cleanedAmount = cleanedAmount.Replace("Rs", "").Replace("US$", "").Trim();

        return (int)float.Parse(cleanedAmount);
    }

    private string[] ParseCsvLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string field = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(field);
                field = "";
            }
            else
            {
                field += c;
            }
        }

        fields.Add(field); // Add the last field
        return fields.ToArray();
    }
}

[System.Serializable]
public class Transaction
{
    public string Type;
    public string Status;
    public DateTime Date;
    public string Payee;
    public string CategoryOrAccount;
    public int Amount;
    public string Note;
    public string Memo;
    public string Number;
    public string Security;
    public string Shares;
    public string PricePerShare;
    public string Commission;
}
