using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;
    private int starsActive;

    public Image[] stars;
    public Text levelText;
    public int level;

    public void Setup(int level)
    {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();

        this.level = level;

        //Decide if the level is active
        if (GameData.instance.saveData.isActive[level - 1])
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }

        //Decide how many stars to activate
        starsActive = GameData.instance.saveData.stars[level - 1];

        for (int i = 0; i < 3; i++)
        {
            if (i < starsActive)
                stars[i].enabled = true;
            else
                stars[i].enabled = false;
        }

        levelText.text = "" + level;

        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    public void ConfirmPanel()
    {
        LevelSelect.instance.confirmPanel.Setup(this.level);
        LevelSelect.instance.ShowConfirmPanel(true);
        SoundManager.instance.PlayRandomSFX();
    }
}
