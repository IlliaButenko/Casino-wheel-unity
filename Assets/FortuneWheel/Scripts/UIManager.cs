using SimpleJSON;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool _isStarted;

    public TMP_Text walletAmount_Text;
    public TMP_Text CoinsDeltaText;         // Pop-up text with wasted or rewarded coins amount
    public TMP_InputField InputCoinValue;        // public InputField InputCoinValue;
    public TMP_Text Errortext;


    private float[] _sectorsAngles;
    public static Globalinitial _global;
    public static APIForm apiform;
    private float _finalAngle;
    private float _startAngle = 0;
    private float _currentLerpRotationTime;
    private int randomFinalAngle;
    private float PreviousCoinsAmount;

    public GameObject IncreaseButton;
    public GameObject ReduceButton;
    public GameObject Circle;
    public Button TurnButton;
    private string Token = "";
    // Rotatable Object with rewards

    BetPlayer _player;

    public float totalAmount;       // For wasted coins animation
    public float TurnCost;
    public float EarnAmount;

    // GameReadyStatus Send
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    void Start()
    {
        InputCoinValue.text = "100";
        PreviousCoinsAmount = totalAmount;

#if UNITY_WEBGL == true && UNITY_EDITOR == false
            GameReady("Ready");
#endif
    }


    void Update()
    {
        TurnWheel();
    }
    public void TurnWheel()
    {
        // Make turn button non interactable if user has not enough money for the turn
        if (!_isStarted)
            return;
        float maxLerpRotationTime = 10f;
        // increment timer once per frame
        _currentLerpRotationTime += Time.deltaTime;
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;

        }
        // Calculate current position using linear interpolation
        float t = _currentLerpRotationTime / maxLerpRotationTime;
        // This formulae allows to speed up at start and speed down at the end of rotation.
        // Try to change this values to customize the speed
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
        Circle.transform.eulerAngles = new Vector3(0, 0, angle);

    }


    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        Token = usersInfo["token"];
        Debug.Log("token=--------" + usersInfo["token"]);
        Debug.Log("amount=------------" + usersInfo["amount"]);
        Debug.Log("userName=------------" + usersInfo["userName"]);
        _player.token = usersInfo["token"];
        _player.username = usersInfo["userName"];

        float i_balance = float.Parse(usersInfo["amount"]);
        walletAmount_Text.text = (i_balance).ToString();
        totalAmount = float.Parse(walletAmount_Text.text);
    }

    public void InputfieldCoinValue_Edited()
    {
        if (float.Parse(string.IsNullOrEmpty(InputCoinValue.text) ? "100" : InputCoinValue.text) <= 100f)
        {
            InputCoinValue.text = "100";
        }
        else if (float.Parse(InputCoinValue.text) >= 100000f)
        {
            InputCoinValue.text = "100000";
        }
        else if (float.Parse(InputCoinValue.text) % 100f != 0)
        {
            float rechange = float.Parse(InputCoinValue.text) - (float.Parse(InputCoinValue.text) % 100);
            InputCoinValue.text = (rechange).ToString();
        }

    }


    public void Reducebtn_Clicked()
    {
        if (_isStarted == false)
        {
            if (float.Parse(InputCoinValue.text) > 100.0 && float.Parse(InputCoinValue.text) <= 1000.0)
            {
                TurnCost = float.Parse(InputCoinValue.text);
                TurnCost -= (float)100.0;
                InputCoinValue.text = (TurnCost).ToString();
                Errortext.text = "";
            }
            else if (float.Parse(InputCoinValue.text) > 1000.0 && float.Parse(InputCoinValue.text) <= 10000.0)
            {
                TurnCost = float.Parse(InputCoinValue.text);
                TurnCost -= (float)1000.0;
                InputCoinValue.text = (TurnCost).ToString();
                Errortext.text = "";
            }
            else if (float.Parse(InputCoinValue.text) > 10000.0 && float.Parse(InputCoinValue.text) <= 100000.0)
            {
                TurnCost = float.Parse(InputCoinValue.text);
                TurnCost -= (float)10000.0;
                InputCoinValue.text = (TurnCost).ToString();
                Errortext.text = "";
            }
        }
    }

    public void Increasebtn_Clicked()
    {
        if (_isStarted == false)
        {
            if (float.Parse(InputCoinValue.text) < float.Parse(walletAmount_Text.text))
            {
                if (float.Parse(InputCoinValue.text) >= 100.0 && float.Parse(InputCoinValue.text) < 1000.0)
                {
                    TurnCost = float.Parse(InputCoinValue.text);
                    TurnCost += (float)100.0;
                    InputCoinValue.text = (TurnCost).ToString();
                    Errortext.text = "";
                }
                else if (float.Parse(InputCoinValue.text) >= 1000.0 && float.Parse(InputCoinValue.text) < 10000.0)
                {
                    TurnCost = float.Parse(InputCoinValue.text);
                    TurnCost += (float)1000.0;
                    InputCoinValue.text = (TurnCost).ToString();
                    Errortext.text = "";
                }
                else if (float.Parse(InputCoinValue.text) >= 10000.0)
                {
                    if (float.Parse(InputCoinValue.text) >= 100000.0)
                    {
                        TurnCost = 100000f;
                        InputCoinValue.text = (TurnCost).ToString();
                        Errortext.text = "";
                    }
                    else
                    {
                        TurnCost = float.Parse(InputCoinValue.text);
                        TurnCost += (float)10000.0;
                        InputCoinValue.text = (TurnCost).ToString();
                        Errortext.text = "";
                    }
                }
            }
            else
            {
                TurnCost = float.Parse(walletAmount_Text.text);
                InputCoinValue.text = (TurnCost).ToString();
                Errortext.text = "";
            }
        }
    }


    public void SpinBtn_Clicked()
    {
        TurnButton.interactable = false;
        TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        totalAmount = Single.Parse(walletAmount_Text.text);
        TurnCost = float.Parse(string.IsNullOrEmpty(InputCoinValue.text) ? "0" : InputCoinValue.text);
        if (TurnCost > 100000f)
        {
            TurnCost = 100000f;
            InputCoinValue.text = "100000";
        }
        else if (totalAmount >= TurnCost)
        {
            StartCoroutine(Server());
        }
        else if (totalAmount >= 100 && totalAmount <= 100000 && totalAmount < TurnCost)
        {
            TurnCost = totalAmount;
            InputCoinValue.text = (TurnCost).ToString();
            StartCoroutine(Server());
        }
        else if (totalAmount < 100)
        {
            Errortext.text = "Total Amount isn't enough.";
        }
    }
    private IEnumerator Server()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", Token);
        form.AddField("betAmount", (TurnCost).ToString());
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/start-Wheel", form);
        yield return www.SendWebRequest();
        _isStarted = true;
        if (www.result != UnityWebRequest.Result.Success)
        {
            Errortext.text = "Server error!";
            yield return new WaitForSeconds(2);
            TurnButton.interactable = true;
            TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
            Errortext.text = "";

        }
        else
        {
            string strData = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            Debug.Log(strData);
            apiform = JsonUtility.FromJson<APIForm>(strData);
            if (apiform.Message == "1")
            {
                Errortext.text = "Bet error!";
                yield return new WaitForSeconds(2);
                TurnButton.interactable = true;
                TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
                Errortext.text = "";
            }
            else if (apiform.Message == "2")
            {
                Errortext.text = "Response Error";
                yield return new WaitForSeconds(2);
                TurnButton.interactable = true;
                TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
                Errortext.text = "";
            }
            else
            {
                walletAmount_Text.text = (totalAmount - TurnCost).ToString();
                totalAmount = float.Parse(walletAmount_Text.text);
                CoinsDeltaText.text = "-" + (TurnCost).ToString();
                StartCoroutine(HideCoinsDelta());
                StartCoroutine(UpdateCoinsAmount());
                int fullCircles = 20;
                _finalAngle = -(fullCircles * 360 + 30 * apiform.Angle + 30);
                _currentLerpRotationTime = 0f;
                yield return new WaitForSeconds(10f);
                _isStarted = false;
                CoinsDeltaText.text = "+" + (apiform.WinMoney).ToString();
                CoinsDeltaText.gameObject.SetActive(true);
                walletAmount_Text.text = (Single.Parse(walletAmount_Text.text) + apiform.WinMoney).ToString();
                totalAmount = float.Parse(walletAmount_Text.text);
                StartCoroutine(UpdateCoinsAmount());
                yield return new WaitForSeconds(2f);
                TurnButton.interactable = true;
                TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
            }
        }
    }
    private IEnumerator UpdateCoinsAmount()
    {
        // Animation for increasing and decreasing of coins amount
        const float seconds = 0.2f;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            walletAmount_Text.text = Mathf.Floor(Mathf.Lerp(PreviousCoinsAmount, totalAmount, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PreviousCoinsAmount = totalAmount;
        walletAmount_Text.text = totalAmount.ToString();
    }
    private IEnumerator HideCoinsDelta()
    {
        yield return new WaitForSeconds(5f);
        CoinsDeltaText.gameObject.SetActive(false);
    }

}
public class BetPlayer
{
    public string username;
    public string token;
}