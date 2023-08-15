using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool allowFail;
    public List<string> tags = new List<string>();
    public ComplexObject complexObject;
    public TMPro.TextMeshProUGUI textElement;
    public GameObject confettiEffects;
    public GameObject WonUI;
    public AudioClip CorrectSound;
    public AudioClip WrongSound;
    public Color blinkColor;

    public string startHint ="Find a ";
    const string highlightColor = "#FFFF00";

    void Awake(){
        instance = this;
    }
    void Start(){
        UpdateText();
    }
    void UpdateText(){
        if(tags.Count > 0){
            textElement.transform.parent.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.3f);
            textElement.text = startHint + "<color=" + highlightColor + ">" + tags[0].ToLower() + "!</color>";
        }else
            textElement.transform.parent.gameObject.SetActive(false);
    }
    private float holdDuration=0f;
    void Update(){
        if (Input.GetMouseButtonDown(0)){
            Invoke("CheckAgain", 0.1f); // to void mistaking a hold for a tap
            holdDuration = 0;
        }
        if(Input.GetMouseButton(0)){
            holdDuration += Time.deltaTime;
        }
        
        if(Input.GetMouseButtonUp(0)){
            CancelInvoke("CheckAgain");
            if(holdDuration < 0.1f) CheckAgain();
        }

        if(Input.GetKeyDown(KeyCode.Space))
            Hint();
    }
    void CheckAgain(){
        if(!Input.GetMouseButton(0)){
            var ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            if (Physics.Raycast(ray, out var hit))
            {
                ClickableObject obj = FindClickableObject(hit.collider);
                if (obj != null)
                    obj.Colorify(hit.point);
                else
                    Debug.Log("Hit nothing important (" + hit.collider.name + ")");
            }
        }
    }
    private bool Hinting = false;
    public void Hint(){
        if(Hinting) return;

        string wantedTag = tags[0];
        ClickableObject obj = null;
        foreach(var comp in complexObject.components){
            if(comp.tag == wantedTag){
                obj = comp;
                break;
            }
        }

        if(obj == null) return;

        foreach(var renderer in obj.components)
            StartCoroutine(Blink(renderer, renderer.material.GetColor("_BaseColor"), blinkColor, 0.165f, 3));

        Hinting = true;
    }
    IEnumerator Blink(MeshRenderer renderer, Color startColor, Color endColor, float duration, int times){
        int count = 0;
        while(count < times){
            var startTime = Time.time;
            var endTime = startTime + duration;
            while (Time.time < endTime){
                float perc = (Time.time - startTime) / duration;
                renderer.material.SetColor("_BaseColor", Color.Lerp(startColor, endColor, perc));
                yield return null;
            }
            yield return new WaitForSecondsRealtime(0.1f);

            startTime = Time.time;
            endTime = startTime + duration;
            while (Time.time < endTime)
            {
                float perc = (Time.time - startTime) / duration;
                renderer.material.SetColor("_BaseColor", Color.Lerp(endColor, startColor, perc));
                yield return null;
            }
            count++;
        }
        Hinting = false;
    }
    public void checkWin(){
        if(tags.Count > 0) return;
        
        Debug.Log("Game over!");
        if(complexObject.GetComponent<Animator>())
            complexObject.GetComponent<Animator>().SetTrigger("Animate");
        Invoke("Confetti", 0.2f);
        Invoke("Won", 2.8f);
    }
    void Confetti(){
        confettiEffects.SetActive(true);
        complexObject.ColorifyAll();
    }
    void Won(){
        WonUI.SetActive(true);
    }
    public ClickableObject FindClickableObject(Collider col){
        return complexObject.FindClickableObject(col);
    }
    public bool clicked(string clickedTag){
        if(tags.Count > 0 && tags[0] == clickedTag){
            tags.RemoveAt(0);
            UpdateText();
            GetComponent<AudioSource>().PlayOneShot(CorrectSound);
            return true;
        }else{
            GetComponent<AudioSource>().PlayOneShot(WrongSound);
            return false;
        }
    }
    public void NextLevel(){
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex+1) % SceneManager.sceneCountInBuildSettings);
    }
}