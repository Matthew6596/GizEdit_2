using UnityEngine;
using UnityEngine.UI;

public class StudBarElement : BarElement
{
    [SerializeField]
    private Image[] progressStuds;

    private Sprite[] studSprites;

    private Image fallbackBg;

    private void Awake()
    {
        bar = GetComponent<Image>();
        fallbackBg = bar.transform.parent.GetComponent<Image>();

        foreach (Image img in progressStuds) img.gameObject.SetActive(false);

        TTResourceManager.OnLoaded.AddListener(() =>
        {
            studSprites = new Sprite[10];
            for(int i=0; i<studSprites.Length; i++)
            {
                Texture2D studTxtr = TTResourceManager.GetMaterial("progress" + i).mainTexture as Texture2D;
                studSprites[i] = Sprite.Create(studTxtr, new Rect(0,0,studTxtr.width,studTxtr.height), Vector2.zero);
                studSprites[i].name = "progress_stud_" + i;
            }

            bar.enabled = false;
            fallbackBg.enabled = false;

            foreach(Image img in progressStuds) img.gameObject.SetActive(true);
        });
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void SetFillAmount(float percent)
    {
        if (TTResourceManager.WorkingGameLoaded)
        {
            int amt = (int)(Mathf.Clamp01(percent)*(progressStuds.Length*9));
            for(int i=0; i<progressStuds.Length; i++)
            {
                int studAmt = Mathf.Min(amt, 9);
                amt -= studAmt;
                progressStuds[i].sprite = studSprites[studAmt];
            }
        }
        else
        {
            base.SetFillAmount(percent);
        }
    }

    public override void SetColor(Color col)
    {
        if (TTResourceManager.WorkingGameLoaded)
        {

        }
        else
        {
            base.SetColor(col);
        }
    }
}
