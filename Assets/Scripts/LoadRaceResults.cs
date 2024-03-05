using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using static System.Environment;
using System;

public class LoadRaceResults : MonoBehaviour
{
    [System.Serializable]
    public class RaceResult
    {
        public string name;
        public List<double> circleTime;
        public double allTime;
    }
    public GameObject resultPrefab; // Ссылка на ваш префаб
    public Transform resultsParent; // Родительский объект для инстанцированных префабов
    public TMP_InputField key; // Родительский объект для инстанцированных префабов

    [System.Serializable]
    public class RaceData
    {
        public List<RaceResult> raceResults = new List<RaceResult>();
    }

    void Start()
    {
        key.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }
    public void ValueChangeCheck()
    {
        LoadResults(key.text);
    }
    private string GetAppDataPath()
    {
        return Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData), "..", "LocalLow");
    }

    public void LoadResults(string fileName)
    {
        string appDataPath = GetAppDataPath();
        string path = Path.Combine(appDataPath, "DigitLab", "DroneRace", $"{fileName}.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            RaceData loadedData = JsonUtility.FromJson<RaceData>(json);

            // Clear previous results
            foreach (Transform child in resultsParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var result in loadedData.raceResults)
            {
                GameObject newResultGO = Instantiate(resultPrefab, resultsParent);
                TMP_Text resultName = newResultGO.transform.GetChild(0).GetComponent<TMP_Text>();
                TMP_Text resultTime = newResultGO.transform.GetChild(1).GetComponent<TMP_Text>();
                TMP_Text resultCircle = newResultGO.transform.GetChild(2).GetComponent<TMP_Text>();

                resultName.text = $"{result.name}";
                resultTime.text = $"Время: {TimeSpan.FromSeconds(result.allTime).ToString(@"mm\:ss\:fff")}";

                resultCircle.text = "";
                for(int i=0; i< result.circleTime.Count;i++)
                {
                    resultCircle.text += $"Круг {i+1}: {TimeSpan.FromSeconds(result.circleTime[i]).ToString(@"mm\:ss\:fff")}\n";
                }
            }
        }
        else
        {
            for (int i = 0; i < resultsParent.childCount; i++)
            {
                Destroy(resultsParent.GetChild(i).gameObject);
            }
        }
    }

}
