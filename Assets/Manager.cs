using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    enum state
    {
        MainMenu,
        OptionsMenu,
        Credits,
        PreLevel,
        Game,
        Win,
        Lose
    }

    [SerializeField]
    Player player;
    [SerializeField]
    Camera camera;
    [SerializeField]
    GameObject timeText;
    [SerializeField]
    GameObject timeText2;
    [SerializeField]
    GameObject menuScreen;
    [SerializeField]
    GameObject optionsScreen;
    [SerializeField]
    GameObject creditsScreen;
    [SerializeField]
    GameObject preLevelScreen;
    [SerializeField]
    GameObject winScreen;
    [SerializeField]
    GameObject loseScreen;
    [SerializeField]
    float levelTimer = 1.02f;
    [SerializeField]
    AudioSource menuMusic;
    [SerializeField]
    AudioSource gameMusic;
    [SerializeField]
    AudioSource clockSound;
    [SerializeField]
    AudioSource glassSound;
    [SerializeField]
    AudioSource synthSound;

    public Dictionary<string, KeyCode> keys;

    GameObject startText;
    GameObject clockText;

    float timer;

    state currentState = state.MainMenu;

    string changingKey;
    GameObject changingButton;

    float curVol;
    float targetVol;

    string oldText;

    float cameraDefault;

    public bool finalWin;

    // Start is called before the first frame update
    void Start()
    {
        keys = new Dictionary<string, KeyCode>();

        keys.Add("up", KeyCode.W);
        keys.Add("down", KeyCode.S);
        keys.Add("left", KeyCode.A);
        keys.Add("right", KeyCode.D);

        timeText.GetComponent<Text>().text = "0.0000";
        timeText.SetActive(false);
        timeText2.GetComponent<Text>().text = "0.0000";
        timeText2.SetActive(false);

        startText = preLevelScreen.transform.Find("StartText").gameObject;
        clockText = preLevelScreen.transform.Find("ClockText").gameObject;

        clockText.SetActive(false);

        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        preLevelScreen.SetActive(false);
        loseScreen.SetActive(false);
        winScreen.SetActive(false);

        curVol = 0;
        targetVol = 0.5f;
        menuMusic.Play();
        menuMusic.volume = curVol;

        cameraDefault = camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == state.MainMenu || currentState == state.OptionsMenu || currentState == state.Credits)
        {
            if(curVol <= targetVol)
            {
                curVol += 0.001f;

                menuMusic.volume = curVol;
            }
            else
            {
                curVol = targetVol;
            }
        }
        if(currentState == state.PreLevel)
        {
            if (curVol >= targetVol)
            {
                curVol -= 0.005f;

                menuMusic.volume = curVol;
            }
            else
            {
                curVol = targetVol;
                menuMusic.Stop();
            }

            if (timer < 0 && Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Mouse0))
            {
                timer = 1;

                startText.SetActive(false);
                clockText.SetActive(true);

                string temp = Mathf.Ceil((timer / 1) * 3).ToString();

                clockText.GetComponent<Text>().text = temp;

                clockSound.Play();

                oldText = temp;
            }
            else if (timer > 0)
            {
                timer -= Time.deltaTime;

                string temp = Mathf.Ceil((timer / 1) * 3).ToString();

                clockText.GetComponent<Text>().text = temp;

                if(temp != oldText)
                {
                    clockSound.Play();
                }

                oldText = temp;

                if (timer <= 0)
                {
                    timeText.SetActive(true);
                    timeText2.SetActive(true);

                    preLevelScreen.SetActive(false);

                    player.gameActive = true;

                    currentState = state.Game;

                    timer = levelTimer;

                    if(!gameMusic.isPlaying)
                    {
                        gameMusic.Play();
                    }
                }
            }
        }
        else if(currentState == state.Game)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                timeText.GetComponent<Text>().text = (timer / levelTimer).ToString("F4");
                timeText2.GetComponent<Text>().text = (timer / levelTimer).ToString("F4");
            }
            else
            {
                loseScreen.SetActive(true);

                player.gameActive = false;

                currentState = state.Lose;

                timer = 0.5f;

                timeText.SetActive(false);
                timeText2.SetActive(false);
            }

            if(timer < 0)
            {
                timeText.GetComponent<Text>().text = "0.0000";
                timeText2.GetComponent<Text>().text = "0.0000";
            }
        }
        else if(currentState == state.Lose || currentState == state.Win)
        {
            if(currentState == state.Win)
            {
                if (timer <= 0.5f)
                {
                    winScreen.SetActive(true);

                    camera.orthographicSize = cameraDefault;

                    player.transform.position = Vector3.zero;
                    camera.transform.position = new Vector3(0, 0, -10);

                    player.gameActive = false;
                }
                else
                {
                    camera.orthographicSize -= 0.05f;
                }
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else if(Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Mouse0))
            {
                player.transform.position = Vector3.zero;
                camera.transform.position = new Vector3(0, 0, -10);

                if (finalWin)
                {
                    Application.Quit();
                }

                timeText.SetActive(false);
                timeText2.SetActive(false);

                loseScreen.SetActive(false);
                winScreen.SetActive(false);
                preLevelScreen.SetActive(true);
                startText.SetActive(false);
                clockText.SetActive(true);

                currentState = state.PreLevel;

                timer = 1;
            }
        }
    }

    public void StartGame()
    {
        timer = -1;

        currentState = state.PreLevel;

        preLevelScreen.SetActive(true);
        startText.SetActive(true);
        clockText.SetActive(false);
        menuScreen.SetActive(false);

        targetVol = 0;
    }

    public void ToOptions()
    {
        currentState = state.OptionsMenu;

        menuScreen.SetActive(false);

        optionsScreen.SetActive(true);
    }

    public void ToCredits()
    {
        currentState = state.Credits;

        menuScreen.SetActive(false);

        creditsScreen.SetActive(true);
    }

    public void ReturnToMenu(GameObject callingScreen)
    {
        currentState = state.MainMenu;

        callingScreen.SetActive(false);

        menuScreen.SetActive(true);
    }

    public void Win()
    {
        Text tempText = winScreen.transform.Find("Text").GetComponent<Text>();
        tempText.text = timer.ToString("F4") + tempText.text;

        currentState = state.Win;

        timer = 3f;

        timeText.SetActive(false);
        timeText2.SetActive(false);

        glassSound.Play();
        synthSound.Play();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChangeKey(string keyKey)
    {
        if (changingKey != null && changingButton != null)
        {
            changingButton.transform.Find("Text").gameObject.GetComponent<Text>().text = keys[changingKey].ToString();
            changingButton.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.08f);
        }

        changingKey = keyKey;
    }

    public void ChangeButton(GameObject button)
    {
        button.transform.Find("Text").gameObject.GetComponent<Text>().text = "Press key";

        button.GetComponent<Image>().color = new Color(0.18f, 0.18f, 0.18f);

        changingButton = button;
    }

    private void OnGUI()
    {
        if(changingKey != null && changingButton != null)
        {
            Event e = Event.current;

            if (e.keyCode != KeyCode.None)
            {
                if (e.keyCode == KeyCode.Escape)
                {
                    changingButton.transform.Find("Text").gameObject.GetComponent<Text>().text = keys[changingKey].ToString();
                    changingButton.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.08f);

                    changingKey = null;
                    changingButton = null;
                }
                else if (e.keyCode != KeyCode.F12)
                {
                    keys[changingKey] = e.keyCode;

                    changingButton.transform.Find("Text").gameObject.GetComponent<Text>().text = e.keyCode.ToString();
                    changingButton.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.08f);

                    changingKey = null;
                    changingButton = null;
                }
            }
        }
    }
}
