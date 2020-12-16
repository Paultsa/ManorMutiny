using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
    public float time;
    public int kills;
    public int wave;
}

[Serializable]
public class LeaderboardEntryMP
{
    public string nameP1;
    public string nameP2;

    public int score;
    //public int scoreP1;
    //public int scoreP2;
    public float time;

    public int kills;
    public int killsP1;
    public int killsP2;

    public int wave;
}

[Serializable]
public class Leaderboard
{
    public List<LeaderboardEntry> entryList = new List<LeaderboardEntry>();
}

[Serializable]
public class LeaderboardMP
{
    public List<LeaderboardEntryMP> entryList = new List<LeaderboardEntryMP>();
}

public class LeaderboardScript : MonoBehaviour
{

    GameObject leaderboardEntryContainer;
    Transform leaderboardEntry;
    Transform leaderboardEntryMP;
    Transform rankColumn;

    Rect entryFieldRect;

    Leaderboard leaderboard;
    LeaderboardMP leaderboardMP;

    bool boardMP = false;

    [SerializeField]
    Button buttonSP;

    [SerializeField]
    Button buttonMP;

    List<Transform> leaderboardEntryFieldList = new List<Transform>();

    bool scoreSortReverse = false;
    bool timeSortReverse = false;
    bool killSortReverse = false;
    bool waveSortReverse = false;

    public void SortEntriesByScore()
    {
        if (boardMP)
        {
            SortEntriesByScoreMP();
        }
        else
        {
            scoreSortReverse = !scoreSortReverse;
            killSortReverse = true;
            waveSortReverse = true;

            leaderboardEntryFieldList.Clear();

            List<GameObject> childList = new List<GameObject>();
            for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
            {
                if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
                {
                    childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
                }
            }

            foreach (GameObject child in childList)
            {
                child.transform.SetParent(null);
                Destroy(child);
            }

            if (scoreSortReverse)
            {
                leaderboard.entryList.Sort((x, y) => x.score.CompareTo(y.score));
            }
            else
            {
                leaderboard.entryList.Sort((x, y) => y.score.CompareTo(x.score));
            }


            if (leaderboard != null)
            {
                foreach (LeaderboardEntry entry in leaderboard.entryList)
                {
                    CreateLeaderboardEntryField(entry);
                }
            }

            RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
            leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * 40);
        }
    }
    public void SortEntriesByScoreMP()
    {
        scoreSortReverse = !scoreSortReverse;
        killSortReverse = true;
        waveSortReverse = true;

        leaderboardEntryFieldList.Clear();

        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
        {
            if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
            {
                childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
            }
        }

        foreach (GameObject child in childList)
        {
            child.transform.SetParent(null);
            Destroy(child);
        }

        if (scoreSortReverse)
        {
            leaderboardMP.entryList.Sort((x, y) => x.score.CompareTo(y.score));
        }
        else
        {
            leaderboardMP.entryList.Sort((x, y) => y.score.CompareTo(x.score));
        }


        if (leaderboardMP != null)
        {
            foreach (LeaderboardEntryMP entry in leaderboardMP.entryList)
            {
                CreateLeaderboardEntryFieldMP(entry);
            }
        }

        RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
        leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * 40);
    }

    public void SortEntriesByTime()
    {
        if (boardMP)
        {
            SortEntriesByTimeMP();
        }
        else
        {
            timeSortReverse = !timeSortReverse;
            scoreSortReverse = true;
            killSortReverse = true;
            waveSortReverse = true;

            leaderboardEntryFieldList.Clear();

            List<GameObject> childList = new List<GameObject>();
            for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
            {
                if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
                {
                    childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
                }
            }

            foreach (GameObject child in childList)
            {
                child.transform.SetParent(null);
                Destroy(child);
            }

            if (timeSortReverse)
            {
                leaderboard.entryList.Sort((x, y) => x.time.CompareTo(y.time));
            }
            else
            {
                leaderboard.entryList.Sort((x, y) => y.time.CompareTo(x.time));
            }


            if (leaderboard != null)
            {
                foreach (LeaderboardEntry entry in leaderboard.entryList)
                {
                    CreateLeaderboardEntryField(entry);
                }
            }

            RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
            leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);
        }
    }
    public void SortEntriesByTimeMP()
    {
        timeSortReverse = !timeSortReverse;
        scoreSortReverse = true;
        killSortReverse = true;
        waveSortReverse = true;

        leaderboardEntryFieldList.Clear();

        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
        {
            if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
            {
                childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
            }
        }

        foreach (GameObject child in childList)
        {
            child.transform.SetParent(null);
            Destroy(child);
        }

        if (timeSortReverse)
        {
            leaderboardMP.entryList.Sort((x, y) => x.time.CompareTo(y.time));
        }
        else
        {
            leaderboardMP.entryList.Sort((x, y) => y.time.CompareTo(x.time));
        }


        if (leaderboardMP != null)
        {
            foreach (LeaderboardEntryMP entry in leaderboardMP.entryList)
            {
                CreateLeaderboardEntryFieldMP(entry);
            }
        }

        RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
        leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);
    }

    public void SortEntriesByKills()
    {
        if (boardMP)
        {
            SortEntriesByKillsMP();
        }
        else
        {
            timeSortReverse = true;
            scoreSortReverse = true;
            killSortReverse = !killSortReverse;
            waveSortReverse = true;

            leaderboardEntryFieldList.Clear();

            List<GameObject> childList = new List<GameObject>();
            for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
            {
                if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
                {
                    childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
                }
            }

            foreach (GameObject child in childList)
            {
                child.transform.SetParent(null);
                Destroy(child);
            }

            if (killSortReverse)
            {
                leaderboard.entryList.Sort((x, y) => x.kills.CompareTo(y.kills));
            }
            else
            {
                leaderboard.entryList.Sort((x, y) => y.kills.CompareTo(x.kills));
            }

            if (leaderboard != null)
            {
                foreach (LeaderboardEntry entry in leaderboard.entryList)
                {
                    CreateLeaderboardEntryField(entry);
                }
            }

            RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
            leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);
        }
    }
    public void SortEntriesByKillsMP()
    {
        timeSortReverse = true;
        scoreSortReverse = true;
        killSortReverse = !killSortReverse;
        waveSortReverse = true;

        leaderboardEntryFieldList.Clear();

        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
        {
            if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
            {
                childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
            }
        }

        foreach (GameObject child in childList)
        {
            child.transform.SetParent(null);
            Destroy(child);
        }

        if (killSortReverse)
        {
            leaderboardMP.entryList.Sort((x, y) => x.kills.CompareTo(y.kills));
        }
        else
        {
            leaderboardMP.entryList.Sort((x, y) => y.kills.CompareTo(x.kills));
        }


        if (leaderboardMP != null)
        {
            foreach (LeaderboardEntryMP entry in leaderboardMP.entryList)
            {
                CreateLeaderboardEntryFieldMP(entry);
            }
        }

        RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
        leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);
    }

    public void SortEntriesByWaves()
    {
        if (boardMP)
        {
            SortEntriesByWavesMP();
        }
        else
        {
            timeSortReverse = true;
            scoreSortReverse = true;
            killSortReverse = true;
            waveSortReverse = !waveSortReverse;

            leaderboardEntryFieldList.Clear();

            List<GameObject> childList = new List<GameObject>();
            for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
            {
                if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
                {
                    childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
                }
            }

            foreach (GameObject child in childList)
            {
                child.transform.SetParent(null);
                Destroy(child);
            }


            if (waveSortReverse)
            {
                leaderboard.entryList.Sort((x, y) => x.wave.CompareTo(y.wave));
            }
            else
            {
                leaderboard.entryList.Sort((x, y) => y.wave.CompareTo(x.wave));
            }

            if (leaderboard != null)
            {
                foreach (LeaderboardEntry entry in leaderboard.entryList)
                {
                    CreateLeaderboardEntryField(entry);
                }
            }

            RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
            leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);
        }
    }
    public void SortEntriesByWavesMP()
    {
        timeSortReverse = true;
        scoreSortReverse = true;
        killSortReverse = true;
        waveSortReverse = !waveSortReverse;

        leaderboardEntryFieldList.Clear();

        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
        {
            if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
            {
                childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
            }
        }

        foreach (GameObject child in childList)
        {
            child.transform.SetParent(null);
            Destroy(child);
        }

        if (waveSortReverse)
        {
            leaderboardMP.entryList.Sort((x, y) => x.wave.CompareTo(y.wave));
        }
        else
        {
            leaderboardMP.entryList.Sort((x, y) => y.wave.CompareTo(x.wave));
        }


        if (leaderboardMP != null)
        {
            foreach (LeaderboardEntryMP entry in leaderboardMP.entryList)
            {
                CreateLeaderboardEntryFieldMP(entry);
            }
        }

        RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
        leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);
    }

    // WARNING: DELETES ALL LEADERBOARD DATA FROM PLAYER PREFS. USE WITH CARE
    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey("Leaderboard");
        Debug.Log("Deleted leaderboard from player prefs");
    }

    // WARNING: DELETES ALL LEADERBOARD DATA FROM PLAYER PREFS. USE WITH CARE
    public void ClearLeaderboardMP()
    {
        PlayerPrefs.DeleteKey("LeaderboardMP");
        Debug.Log("Deleted leaderboardMP from player prefs");
    }


    // WARNING!! CLEARS ALL EXISTING ENTRIES. For debug use
    public void GenerateLeaderboardEntries()
    {
        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
        {
            if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
            {
                childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
            }
        }

        foreach(GameObject child in childList)
        {
            child.transform.SetParent(null);
            Destroy(child);
        }

        leaderboardEntryFieldList.Clear();
        leaderboard.entryList.Clear();

        for (int i = 0; i < UnityEngine.Random.Range(15, 40); i++)
        {
            LeaderboardEntry newEntry = new LeaderboardEntry { name = "player " + UnityEngine.Random.Range(0, 99).ToString(), time = UnityEngine.Random.Range(0, 9999), kills = UnityEngine.Random.Range(0, 100), wave = UnityEngine.Random.Range(0, 30) };
            leaderboard.entryList.Add(newEntry);
        }

        foreach (LeaderboardEntry entry in leaderboard.entryList)
        {
            CreateLeaderboardEntryField(entry);
        }

        RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
        leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);

        Debug.Log("Generated new random entries");
    }
    public void GenerateLeaderboardEntriesMP()
    {
        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < leaderboardEntryContainer.transform.childCount; i++)
        {
            if (leaderboardEntryContainer.transform.GetChild(i).name.Contains("(Clone)"))
            {
                childList.Add(leaderboardEntryContainer.transform.GetChild(i).gameObject);
            }
        }

        foreach (GameObject child in childList)
        {
            child.transform.SetParent(null);
            Destroy(child);
        }

        leaderboardEntryFieldList.Clear();
        leaderboardMP.entryList.Clear();

        for (int i = 0; i < UnityEngine.Random.Range(5, 40); i++)
        {
            LeaderboardEntryMP newEntry = new LeaderboardEntryMP { nameP1 = "player " + UnityEngine.Random.Range(0, 99).ToString(), nameP2 = "player " + UnityEngine.Random.Range(0, 99).ToString(), time = UnityEngine.Random.Range(0, 9999), kills = UnityEngine.Random.Range(0, 100), wave = UnityEngine.Random.Range(0, 30) };
            leaderboardMP.entryList.Add(newEntry);
        }

        foreach (LeaderboardEntryMP entry in leaderboardMP.entryList)
        {
            CreateLeaderboardEntryFieldMP(entry);
        }

        RectTransform rect = leaderboardEntryContainer.transform.GetComponent<RectTransform>();
        leaderboardEntryContainer.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.sizeDelta.x, (leaderboardEntryContainer.transform.childCount - 2) * entryFieldRect.height);

        Debug.Log("Generated new random MP entries");
    }

    void CreateLeaderboardEntryField(LeaderboardEntry entry)
    {
        Transform entryField = Instantiate(leaderboardEntry, leaderboardEntryContainer.transform);
        RectTransform entryRect = entryField.transform.GetComponent<RectTransform>();

        entryRect.anchoredPosition = new Vector2(0, -(entryRect.rect.height / 2f) - entryRect.rect.height * leaderboardEntryFieldList.Count);

        if (leaderboardEntryFieldList.Count % 2 == 1)
        {
            entryField.transform.Find("LeaderboardEntryBG").GetComponent<Image>().color -= new Color(29f / 255f, 29f / 255f, 29f / 255f, 0); // Every other color is dark tinted version of the original color
        }

        entryField.transform.Find("RankNro").GetComponent<TextMeshProUGUI>().text = (leaderboardEntryFieldList.Count + 1).ToString();
        entryField.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = entry.name;

        entryField.transform.Find("PlayerTime").GetComponent<TextMeshProUGUI>().text = Mathf.FloorToInt(entry.time / 60f).ToString() + "m:" + ((Mathf.RoundToInt(entry.time) % 60)).ToString() + "s";
        //entryField.transform.Find("PlayerScore").GetComponent<TextMeshProUGUI>().text = entry.score.ToString();
        entryField.transform.Find("PlayerKills").GetComponent<TextMeshProUGUI>().text = entry.kills.ToString();
        entryField.transform.Find("PlayerWave").GetComponent<TextMeshProUGUI>().text = entry.wave.ToString();

        //entryField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (leaderboardEntryFieldList.Count + 1).ToString();
        //entryField.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.name;
        //entryField.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = entry.score.ToString();
        //entryField.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = entry.kills.ToString();
        //entryField.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = entry.wave.ToString();

        entryField.gameObject.SetActive(true);

        leaderboardEntryFieldList.Add(entryField);

    }
    void CreateLeaderboardEntryFieldMP(LeaderboardEntryMP entry)
    {
        //Debug.Log("Creating a new entryfield(MP)");
        Transform entryField = Instantiate(leaderboardEntryMP, leaderboardEntryContainer.transform);
        RectTransform entryRect = entryField.transform.GetComponent<RectTransform>();

        entryRect.anchoredPosition = new Vector2(0, -(entryRect.rect.height / 2f) - entryRect.rect.height * leaderboardEntryFieldList.Count);

        if (leaderboardEntryFieldList.Count % 2 == 1)
        {
            entryField.transform.Find("LeaderboardEntryBG").GetComponent<Image>().color -= new Color(29f / 255f, 29f / 255f, 29f / 255f, 0); // Every other color is dark tinted version of the original color
        }

        entryField.transform.Find("RankNro").GetComponent<TextMeshProUGUI>().text = (leaderboardEntryFieldList.Count + 1).ToString();
        entryField.transform.Find("PlayerNames").GetComponent<TextMeshProUGUI>().text = entry.nameP1 + " & " + entry.nameP2;
        entryField.transform.Find("PlayerTime").GetComponent<TextMeshProUGUI>().text = Mathf.FloorToInt(entry.time / 60f).ToString() + "m:" + ((Mathf.RoundToInt(entry.time) % 60)).ToString() + "s";
        //entryField.transform.Find("PlayerScore").GetComponent<TextMeshProUGUI>().text = entry.score.ToString();
        entryField.transform.Find("PlayerKills").GetComponent<TextMeshProUGUI>().text = entry.kills.ToString();
        entryField.transform.Find("PlayerWave").GetComponent<TextMeshProUGUI>().text = entry.wave.ToString();

        //entryField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (leaderboardEntryFieldList.Count + 1).ToString();
        //entryField.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.name;
        //entryField.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = entry.score.ToString();
        //entryField.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = entry.kills.ToString();
        //entryField.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = entry.wave.ToString();

        entryField.gameObject.SetActive(true);

        leaderboardEntryFieldList.Add(entryField);

    }

    public void AddLeaderboardEntry(LeaderboardEntry newEntry)
    {
        string oldBoardJson = PlayerPrefs.GetString("Leaderboard");

        if (oldBoardJson.Length > 0)
        {
            leaderboard = JsonUtility.FromJson<Leaderboard>(oldBoardJson);
        }
        else
        {
            leaderboard = new Leaderboard();
        }
       

        leaderboard.entryList.Add(newEntry);

        string newBoardJson = JsonUtility.ToJson(leaderboard);
        PlayerPrefs.SetString("Leaderboard", newBoardJson);
        PlayerPrefs.Save();

        Debug.Log("Leaderboard entry saved");
    }
    public void AddLeaderboardEntryMP(LeaderboardEntryMP newEntry)
    {
        string oldBoardJson = PlayerPrefs.GetString("LeaderboardMP");

        if(oldBoardJson.Length > 0)
        {
            leaderboardMP = JsonUtility.FromJson<LeaderboardMP>(oldBoardJson);
        }
        else
        {
            leaderboardMP = new LeaderboardMP();
        }

        leaderboardMP.entryList.Add(newEntry);

        string newBoardJson = JsonUtility.ToJson(leaderboard);
        PlayerPrefs.SetString("LeaderboardMP", newBoardJson);
        PlayerPrefs.Save();

        Debug.Log("Leaderboard entry(MP) saved");
    }


    public void SwitchBoardMP()
    {
        buttonMP.GetComponent<Image>().color = new Color(80f / 255f, 12f / 255f, 0, 255f / 255f);
        buttonSP.GetComponent<Image>().color = new Color(111f / 255f, 12f / 255f, 0, 255f / 255f);
        boardMP = true;
        waveSortReverse = true;
        SortEntriesByWavesMP();
    }

    public void SwitchBoardSP()
    {
        buttonSP.GetComponent<Image>().color = new Color(80f / 255f, 12f / 255f, 0, 255f / 255f);
        buttonMP.GetComponent<Image>().color = new Color(111f / 255f, 12f / 255f, 0, 255f / 255f);
        boardMP = false;
        waveSortReverse = true;
        SortEntriesByWaves();
    }

    // WARNING!!! OVERRIDES LAST SAVED BOARD! For debug use
    public void SaveCurrentBoard()
    {
        string newBoardJson = JsonUtility.ToJson(leaderboard);

        PlayerPrefs.SetString("Leaderboard", newBoardJson);
        PlayerPrefs.Save();
        Debug.Log("Saved current board to player prefs");
    }

    public void SaveCurrentBoardMP()
    {
        string newBoardJson = JsonUtility.ToJson(leaderboardMP);

        PlayerPrefs.SetString("LeaderboardMP", newBoardJson);
        PlayerPrefs.Save();
        Debug.Log("Saved current board to player prefs(MP)");
    }

    // Start is called before the first frame update
    void Start()
    {
        
        string leaderboardJson = PlayerPrefs.GetString("Leaderboard");
        string leaderboardMPJson = PlayerPrefs.GetString("LeaderboardMP");

        if (leaderboardJson.Length > 0)
        {
            Debug.Log("Existing leaderboard found");
            leaderboard = JsonUtility.FromJson<Leaderboard>(leaderboardJson);
        }
        else
        {
            leaderboard = new Leaderboard();
        }

        if (leaderboardMPJson.Length > 0)
        {
            Debug.Log("Existing leaderboard(MP) found");
            leaderboardMP = JsonUtility.FromJson<LeaderboardMP>(leaderboardMPJson);
        }
        else
        {
            leaderboardMP = new LeaderboardMP();
        }
        
        
        leaderboardEntryContainer = GameObject.Find("LeaderboardEntryContainer");
        leaderboardEntry = GameObject.Find("LeaderboardEntry").transform;
        leaderboardEntryMP = GameObject.Find("LeaderboardEntryMP").transform;
        leaderboardEntry.gameObject.SetActive(false);
        leaderboardEntryMP.gameObject.SetActive(false);

        entryFieldRect = leaderboardEntry.GetComponent<RectTransform>().rect;

        buttonSP.GetComponent<Image>().color = new Color(80f / 255f, 12f / 255f, 0, 255f / 255f);
        buttonMP.GetComponent<Image>().color = new Color(111f / 255f, 12f / 255f, 0, 255f / 255f);

        waveSortReverse = true;
        SortEntriesByWaves();
    }
}
