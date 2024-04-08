using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainContainer_RT;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;  //images taken initially

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;     //class to store total images
    [SerializeField]
    private List<SlotImage> Tempimages;     //class to store the result matrix

    [Header("Slots Objects")]
    [SerializeField]
    private GameObject[] Slot_Objects; 
    [Header("Slots Elements")]
    [SerializeField]
    private LayoutElement[] Slot_Elements;  

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;

    [Header("Line Button Objects")]
    [SerializeField]
    private List<GameObject> StaticLine_Objects;

    [Header("Line Button Texts")]
    [SerializeField]
    private List<TMP_Text> StaticLine_Texts;

    private Dictionary<int, string> x_string = new Dictionary<int, string>();
    private Dictionary<int, string> y_string = new Dictionary<int, string>();

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Bonus_Sprite;
    [SerializeField]
    private Sprite[] FreeSpin_Sprite;
    [SerializeField]
    private Sprite[] Jackpot_Sprite;
    [SerializeField]
    private Sprite[] MajorBlondeMan_Sprite;
    [SerializeField]
    private Sprite[] MajorBlondyGirl_Sprite;
    [SerializeField]
    private Sprite[] MajorDarkMan_Sprite;
    [SerializeField]
    private Sprite[] MajorGingerGirl_Sprite;
    [SerializeField]
    private Sprite[] RuneFehu_Sprite;
    [SerializeField]
    private Sprite[] RuneGebo_Sprite;
    [SerializeField]
    private Sprite[] RuneMannaz_Sprite;
    [SerializeField]
    private Sprite[] RuneOthala_Sprite;
    [SerializeField]
    private Sprite[] RuneRaidho_Sprite;
    [SerializeField]
    private Sprite[] Scatter_Sprite;
    [SerializeField]
    private Sprite[] Wild_Sprite;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private TMP_Text Lines_text;
    [SerializeField]
    private TMP_Text TotalWin_text;
    [SerializeField]
    private Button AutoSpin_Button;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    private Button BetPlus_Button;
    [SerializeField]
    private Button BetMinus_Button;
    [SerializeField]
    private Button LinePlus_Button;
    [SerializeField]
    private Button LineMinus_Button;

    int tweenHeight = 0;  //calculate the height at which tweening is done

    [SerializeField]
    private GameObject Image_Prefab;    //icons prefab

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private Tweener tweener1;
    private Tweener tweener2;
    private Tweener tweener3;
    private Tweener tweener4;
    private Tweener tweener5;

    [SerializeField]
    private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing

    private int numberOfSlots = 5;          //number of columns

    [SerializeField]
    int verticalVisibility = 3;

    [SerializeField]
    private SocketIOManager SocketManager;
    //Coroutine AutoSpinRoutine = null;

    private void Start()
    {

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(StartSlots);

        if (BetPlus_Button) BetPlus_Button.onClick.RemoveAllListeners();
        if (BetPlus_Button) BetPlus_Button.onClick.AddListener(delegate { ChangeBet(true); });
        if (BetMinus_Button) BetMinus_Button.onClick.RemoveAllListeners();
        if (BetMinus_Button) BetMinus_Button.onClick.AddListener(delegate { ChangeBet(false); });

        if (LinePlus_Button) LinePlus_Button.onClick.RemoveAllListeners();
        if (LinePlus_Button) LinePlus_Button.onClick.AddListener(delegate { ChangeLine(true); });
        if (LineMinus_Button) LineMinus_Button.onClick.RemoveAllListeners();
        if (LineMinus_Button) LineMinus_Button.onClick.AddListener(delegate { ChangeLine(false); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        //if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        //if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);
        //numberOfSlots = 5;
        //PopulateInitalSlots(numberOfSlots);
        //FetchLines();
    }

    //Fetch Lines from backend
    internal void FetchLines(string x_value, string y_value, int LineID, int count)
    {
        x_string.Add(LineID, x_value);
        y_string.Add(LineID, y_value);
        StaticLine_Texts[count].text = LineID.ToString();
        StaticLine_Objects[count].SetActive(true);
    }

    //Generate Static Lines from button hovers
    internal void GenerateStaticLine(TMP_Text LineID_Text)
    {
        DestroyStaticLine();
        int LineID = 1;
        try
        {
            LineID = int.Parse(LineID_Text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Exception while parsing " + e.Message);
        }
        List<int> x_points = null;
        List<int> y_points = null;
        x_points = x_string[LineID]?.Split(',')?.Select(Int32.Parse)?.ToList();
        y_points = y_string[LineID]?.Split(',')?.Select(Int32.Parse)?.ToList();
        PayCalculator.GeneratePayoutLinesBackend(x_points, y_points, x_points.Count, true);
    }

    //Destroy Static Lines from button hovers
    internal void DestroyStaticLine()
    {
        PayCalculator.ResetStaticLine();
    }

    private void MaxBet()
    {
        if (TotalBet_text) TotalBet_text.text = "99999";
    }

    private void ChangeLine(bool IncDec)
    {
        double currentline = 1;
        try
        {
            currentline = double.Parse(Lines_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("parse error " + e);
        }
        if (IncDec)
        {
            if (currentline < 20)
            {
                currentline += 1;
            }
            else
            {
                currentline = 20;
            }

            if (currentline > 20)
            {
                currentline = 20;
            }
        }
        else
        {
            if (currentline > 1)
            {
                currentline -= 1;
            }
            else
            {
                currentline = 1;
            }

            if (currentline < 1)
            {
                currentline = 1;
            }
        }

        if (Lines_text) Lines_text.text = currentline.ToString();

    }

    private void ChangeBet(bool IncDec)
    {
        double currentbet = 0;
        try
        {
            currentbet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("parse error " + e);
        }
        if (IncDec)
        {
            if (currentbet < 99999)
            {
                currentbet += 100;
            }
            else
            {
                currentbet = 99999;
            }

            if (currentbet > 99999)
            {
                currentbet = 99999;
            }
        }
        else
        {
            if (currentbet > 0)
            {
                currentbet -= 100;
            }
            else
            {
                currentbet = 0;
            }

            if (currentbet < 0)
            {
                currentbet = 0;
            }
        }

        if (TotalBet_text) TotalBet_text.text = currentbet.ToString();
    }


    //just for testing purposes delete on production
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SlotStart_Button.interactable)
        {
            StartSlots();
        }
    }

    //populate the slots with the values recieved from backend
    internal void PopulateInitalSlots(int number, List<int> myvalues)
    {
        PopulateSlot(myvalues, number);
    }

    //reset the layout after populating the slots
    internal void LayoutReset(int number)
    {
        if (Slot_Elements[number]) Slot_Elements[number].ignoreLayout = true;
        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    private void PopulateSlot(List<int> values , int number)
    {
        if (Slot_Objects[number]) Slot_Objects[number].SetActive(true);
        for(int i = 0; i<values.Count; i++)
        {
            GameObject myImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(myImg.GetComponent<Image>());
            images[number].slotImages[i].sprite = myImages[values[i]];
            PopulateAnimationSprites(images[number].slotImages[i].gameObject.GetComponent<ImageAnimation>(), values[i]);
        }
        for (int k = 0; k < 2; k++)
        {
            GameObject mylastImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(mylastImg.GetComponent<Image>());
            images[number].slotImages[images[number].slotImages.Count - 1].sprite = myImages[values[k]];
            PopulateAnimationSprites(images[number].slotImages[images[number].slotImages.Count - 1].gameObject.GetComponent<ImageAnimation>(), values[k]);
        }
        if (mainContainer_RT) LayoutRebuilder.ForceRebuildLayoutImmediate(mainContainer_RT);
        tweenHeight = (values.Count * IconSizeFactor) - 280;
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        switch(val)
        {
            case 0:
                for (int i = 0; i < Bonus_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Bonus_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
            case 1:
                for (int i = 0; i < FreeSpin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(FreeSpin_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
            case 2:
                for (int i = 0; i < Jackpot_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Jackpot_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
            case 3:
                for (int i = 0; i < MajorBlondeMan_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(MajorBlondeMan_Sprite[i]);
                }
                break;
            case 4:
                for (int i = 0; i < MajorBlondyGirl_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(MajorBlondyGirl_Sprite[i]);
                }
                break;
            case 5:
                for (int i = 0; i < MajorDarkMan_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(MajorDarkMan_Sprite[i]);
                }
                break;
            case 6:
                for (int i = 0; i < MajorGingerGirl_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(MajorGingerGirl_Sprite[i]);
                }
                break;
            case 7:
                for (int i = 0; i < RuneFehu_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(RuneFehu_Sprite[i]);
                }
                break;
            case 8:
                for (int i = 0; i < RuneGebo_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(RuneGebo_Sprite[i]);
                }
                break;
            case 9:
                for (int i = 0; i < RuneMannaz_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(RuneMannaz_Sprite[i]);
                }
                break;
            case 10:
                for (int i = 0; i < RuneOthala_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(RuneOthala_Sprite[i]);
                }
                break;
            case 11:
                for (int i = 0; i < RuneRaidho_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(RuneRaidho_Sprite[i]);
                }
                break;
            case 12:
                for (int i = 0; i < Scatter_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Scatter_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
            case 13:
                for (int i = 0; i < Wild_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Wild_Sprite[i]);
                }
                animScript.AnimationSpeed = 30f;
                break;
        }
    }

    //starts the spin process
    private void StartSlots()
    {
        if (SlotStart_Button) SlotStart_Button.interactable = false;
        if (TempList.Count > 0) 
        {
            StopGameAnimation();
        }
        PayCalculator.ResetLines();
        StartCoroutine(TweenRoutine());
        for (int i = 0; i < Tempimages.Count; i++)
        {
            Tempimages[i].slotImages.Clear();
            Tempimages[i].slotImages.TrimExcess();
        }
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine()
    {
        if (numberOfSlots >= 1)
        {
            InitializeTweening1(Slot_Transform[0]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 2)
        {
            InitializeTweening2(Slot_Transform[1]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 3)
        {
            InitializeTweening3(Slot_Transform[2]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 4)
        {
            InitializeTweening4(Slot_Transform[3]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 5)
        {
            InitializeTweening5(Slot_Transform[4]);
        }
        SocketManager.AccumulateResult();
        yield return new WaitForSeconds(0.5f);
        List<int> resultnum = SocketManager.tempresult.StopList?.Split(',')?.Select(Int32.Parse)?.ToList();
        if (numberOfSlots >= 1)
        {
            yield return StopTweening1(resultnum[0]+3, Slot_Transform[0]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 2)
        {
            yield return StopTweening2(resultnum[1]+3, Slot_Transform[1]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 3)
        {
            yield return StopTweening3(resultnum[2]+3, Slot_Transform[2]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 4)
        {
            yield return StopTweening4(resultnum[3]+3, Slot_Transform[3]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 5)
        {
            yield return StopTweening5(resultnum[4]+3, Slot_Transform[4]);
        }
        yield return new WaitForSeconds(0.3f);
        GenerateMatrix(SocketManager.tempresult.StopList);
        CheckPayoutLineBackend(SocketManager.tempresult.resultLine, SocketManager.tempresult.x_animResult, SocketManager.tempresult.y_animResult);
        KillAllTweens();
        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects) 
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        temp.StartAnimation();
        TempList.Add(temp);
    }

    //stop the icons animation
    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
        }
    }

    //generate the payout lines generated 
    private void CheckPayoutLineBackend(List<int> LineId, List<string> x_AnimString, List<string> y_AnimString)
    {
        List<int> x_points = null;
        List<int> y_points = null;
        List<int> x_anim = null;
        List<int> y_anim = null;

        for (int i = 0; i < LineId.Count; i++)
        {
            x_points = x_string[LineId[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();
            y_points = y_string[LineId[i]]?.Split(',')?.Select(Int32.Parse)?.ToList();
            PayCalculator.GeneratePayoutLinesBackend(x_points, y_points, x_points.Count);
        }

        for (int i = 0; i < x_AnimString.Count; i++)
        {
            x_anim = x_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
            y_anim = y_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

            for (int k = 0; k < x_anim.Count; k++)
            {
                StartGameAnimation(Tempimages[x_anim[k]].slotImages[y_anim[k]].gameObject);
            }
        }
    }

    //generate the result matrix
    private void GenerateMatrix(string stopList)
    {
        List<int> numbers = stopList?.Split(',')?.Select(Int32.Parse)?.ToList();

        for (int i = 0; i < numbers.Count; i++)
        {
            for (int s = 0; s < verticalVisibility; s++)
            {
                Tempimages[i].slotImages.Add(images[i].slotImages[(images[i].slotImages.Count - (numbers[i]+3)) + s]);
            }
        }
    }

    #region TweeningCode
    private void InitializeTweening1(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener1 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener1.Play();
    }
    private void InitializeTweening2(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener2 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener2.Play();
    }
    private void InitializeTweening3(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener3 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener3.Play();
    }
    private void InitializeTweening4(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener4 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener4.Play();
    }
    private void InitializeTweening5(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener5 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener5.Play();
    }

    private IEnumerator StopTweening1(int reqpos, Transform slotTransform)
    {
        tweener1.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener1 = slotTransform.DOLocalMoveY(-tweenpos , 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener1 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening2(int reqpos, Transform slotTransform)
    {
        tweener2.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener2 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener2 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening3(int reqpos, Transform slotTransform)
    {
        tweener3.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener3 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener3 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening4(int reqpos, Transform slotTransform)
    {
        tweener4.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener4 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener4 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening5(int reqpos, Transform slotTransform)
    {
        tweener5.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener5 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener5 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }

    private void KillAllTweens()
    {
        tweener1.Kill();
        tweener2.Kill();
        tweener3.Kill();
        tweener4.Kill();
        tweener5.Kill();
    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}
