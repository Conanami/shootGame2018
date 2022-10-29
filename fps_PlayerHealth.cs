using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
//判断死啊什么的都在这里
public class fps_PlayerHealth : MonoBehaviour
{
    public bool isDead;
    public float resetAfterDeathTime = 5.0f;    //死了以后等多久场景重置
    public AudioClip deathClip;
    public AudioClip damageClip;
    public float maxHp = 100;
    public float hp = 100;
    public float recoverSpeed = 1;

    private float timer = 0;
    private FadeInOut fader;

    private ColorCorrectionCurves colorCurves;   //相机逐渐变成黑白

    void Start()
    {
        hp = maxHp;
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
        colorCurves = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<ColorCorrectionCurves>();
        BleedBehavior.BloodAmount = 0;
    }

    void Update()
    {
        if (!isDead)
            HpRecover();
        if (hp <= 0)
            if (!isDead)
                PlayerDead();
            else
                ResetLevel();
    }

    private void HpRecover()
    {
        hp += recoverSpeed * Time.deltaTime;
        if (hp > maxHp)
            hp = maxHp;
    }
    public void TakeDamage(float damage)
    {
        if (isDead)
            return;
        AudioSource.PlayClipAtPoint(damageClip, transform.position);
        BleedBehavior.BloodAmount += Mathf.Clamp01(damage / hp);
        hp -= damage;
    }

    public void DisableInput()   //死的时候,或者过关的时候调用
    {
        transform.Find("FP_Camera/Weapon_Camera").gameObject.SetActive(false);
        this.GetComponent<fps_PlayerControl>().enabled = false;
        this.GetComponent<AudioSource>().enabled = false;
        this.GetComponent<fps_FpInput>().enabled = false;
        //UI也不显示了
        if (GameObject.Find("Canvas") != null)
            GameObject.Find("Canvas").SetActive(false);
        //相机也不让动了
        colorCurves.gameObject.GetComponent<fps_FPCamera>().enabled = false;
    }

    public void PlayerDead()  //玩家死了，要做的事情
    {
        isDead = true;
        colorCurves.enabled = true;  //颜色变黑白
        DisableInput();
        AudioSource.PlayClipAtPoint(deathClip, transform.position);

    }
    public void ResetLevel()
    {
        timer += Time.deltaTime;
        colorCurves.saturation -= Time.deltaTime / 2;
        colorCurves.saturation = Mathf.Max(0, colorCurves.saturation);
        if (timer >= resetAfterDeathTime)
            fader.EndScene();
    }
}
