using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;


    public static string playerName;
    public InputField pName;

    public static string highScoreName;
    public static int highScore;

    
    // Start is called before the first frame update
    void Start()
    {
        Load();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        
        ScoreText.text = playerName + ": 0";
        HighScoreText.text = "High Score - " + highScoreName + ": " + highScore;
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = playerName+": "+ m_Points;
        if (m_Points > highScore)
        {
            highScoreName = playerName;
            highScore = m_Points;
            HighScoreText.text = "High Score - " + highScoreName + ": " + highScore;
        }
    }

    public void GameOver()
    {
        Save();
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
    public void StartGame()
    {
        playerName = pName.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            SceneManager.LoadScene(1);
        }
       
    }
    public void Save()
    {
        Data data = new Data();
        data.name = highScoreName;
        data.score = highScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/save_1.sav", json);

    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/save_1.sav";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(json);

            highScore = data.score;
            highScoreName = data.name;
        }

    }

    [System.Serializable]
    class Data
    {
        public int score;
        public string name;
    }
}
