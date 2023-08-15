using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TutorialStep {Rotate=0, Zoom=1, Done=2};
public class TutorialManager : MonoBehaviour
{
    private TutorialStep m_tutorialStep;

    public TutorialStep tutorialStep {
        get {return m_tutorialStep;}
        set {
            m_tutorialStep = value;
            PlayerPrefs.SetInt("TutorialStep", (int) value);
            HandleTutorial();
        }
    }
    public GameObject RotateTutorial;
    public GameObject ZoomTutorial;
    void Start(){
        tutorialStep = (TutorialStep)PlayerPrefs.GetInt("TutorialStep", 0);
        HandleTutorial();
    }

    public void HandleTutorial(){
        if(tutorialStep == TutorialStep.Zoom){
            RotateTutorial.SetActive(false);
            ZoomTutorial.SetActive(true);
        }else if(tutorialStep == TutorialStep.Rotate){
            ZoomTutorial.SetActive(false);
            RotateTutorial.SetActive(true);
        }else if(tutorialStep == TutorialStep.Done){
            ZoomTutorial.SetActive(false);
            RotateTutorial.SetActive(false);
        }
    }
}
