using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public TMPro.TMP_InputField numberTest;
    public GameObject PanelQuit;
    public GameObject PanelFirst;
    public GameObject PanelLast;

    public Button StartCycleButton;
    public Button[] SelectPlayerButton;
    public Toggle ShareButton;
    public Button HomeButton;

    public void initTest()
    {
        Debug.Log("Init Test");
        Debug.Log(numberTest.text);
        if (int.Parse(numberTest.text) > 0)
        {
            StartCoroutine(InitTest());
        }
    }
    IEnumerator InitTest()
    {
        PanelQuit.SetActive(false);
        PanelFirst.SetActive(true);
        for (byte i = 0; i < int.Parse(numberTest.text); i++)
        {
            Debug.Log("Test number: " + i);
            StartCycleButton.onClick.Invoke();
            yield return new WaitForSeconds(2f);
            byte selectedPlayer = (byte)Random.Range(0, SelectPlayerButton.Length);
            SelectPlayerButton[selectedPlayer].onClick.Invoke();
            yield return new WaitForSeconds(40f);
            while(PanelLast.activeSelf==false)
            {
                yield return new WaitForSeconds(2f);
            }
            if (Random.Range(0, 2) == 0)
            {
                ShareButton.isOn = true;
            }
            else
            {
                ShareButton.isOn = false;
            }
            yield return new WaitForSeconds(1f);
            HomeButton.onClick.Invoke();
            yield return new WaitForSeconds(5f);
        }
        yield return null;
    }
}
