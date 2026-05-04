using UnityEngine;

public class TrainingStepManager : MonoBehaviour
{
    public string TrainingStep;

    public GameObject TitleSign;
    public GameObject Step1Sign;
    public GameObject Step2Sign;
    public GameObject Step3Sign;
    public GameObject Step4Sign;

    public GameObject YellowArea;

    public GameObject SmokeFX;
    public GameObject FireFX;

    void Start(){
        TrainingStep = "Title"; // Step 0 TitleScreen
    }

    // Step 1: Check for sticks in yellow area

    // Step 2: Check for spark in yellow area

    // Step 3: Check for fan movment in yellow area

    // Step 4: Start Fire!


    public void StartStep1(){
        HideSign(TitleSign);
        ShowSign(Step1Sign);
        TrainingStep = "Step1";
    }
    public void StartStep2(){
        HideSign(Step1Sign);
        ShowSign(Step2Sign);
        TrainingStep = "Step2";
    }
    public void StartStep3(){
        HideSign(Step2Sign);
        ShowSign(Step3Sign);
        TrainingStep = "Step3";
    }
    public void StartStep4(){
        HideSign(Step3Sign);
        ShowSign(Step4Sign);
        TrainingStep = "Step4";
    }

    public void ShowSign(GameObject _sign1){
        _sign1.SetActive(true);

    }
    public void HideSign(GameObject _sign2){
        _sign2.SetActive(false);
    }
}
