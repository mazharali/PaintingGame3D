using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class ClickableObject
{
    public string tag;
    public bool Colored = false;
    public MeshRenderer[] components;
    public float effectDuration = 0.75f;
    public float scaleRatio = 0.3f;
    public Vector2 swipeDirection;

    public bool isIn(Collider child){
        foreach (var col in components)
            if(col.GetComponent<Collider>() == child) return true;
        
        return false;
    }
    public void Colorify(Vector3 center){
        if (Colored) return;

        if (GameManager.instance.tags.Count != 0 && (GameManager.instance.tags[0] != tag && !GameManager.instance.allowFail)) return;

        Vector3 scalePivot = averagePosition(components);
        foreach(var renderer in components){
            foreach(var mat in renderer.materials){
                float startTime = Time.time;
                if(GameManager.instance.tags.Count > 0 && GameManager.instance.tags[0] != tag){
                    GameManager.instance.StartCoroutine(AnimateFail(mat, startTime, center));
                }else{
                    mat.SetInt("_Reversed", 0);
                    mat.SetInt("_Coloring", 1);
                    mat.SetVector("_RippleCenter", center);
                    mat.SetFloat("_RippleStartTime", startTime);
                }
            }
            
            DOVirtual.DelayedCall(effectDuration, () => {
                ScaleAround(renderer.transform, scalePivot);
            });
        }
        GameManager.instance.clicked(tag);
        DOVirtual.DelayedCall(effectDuration, ()=>{
            // Manage score
            Debug.Log(tag + " clicked");
            GameManager.instance.checkWin();
        });

        // Stop next time
        Colored = true;
    }
    public IEnumerator AnimateFail(Material mat, float startTime, Vector3 center){
        mat.SetInt("_Reversed", 1);
        mat.SetInt("_Coloring", 1);
        mat.SetVector("_RippleCenter", center);
        mat.SetFloat("_RippleStartTime", startTime);
        mat.SetFloat("_RippleTime", startTime);
        float duration = effectDuration/2;

        float counter = 0;
        while (counter < duration){
            counter += Time.deltaTime;
            float colorTime = counter / duration;

            //Change color
            mat.SetFloat("_RippleTime", Mathf.Lerp(startTime, startTime + duration, colorTime));
            //Wait for a frame
            yield return null;
        }
        counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float colorTime = counter / duration;

            //Change color
            mat.SetFloat("_RippleTime", Mathf.Lerp(startTime + duration, startTime, colorTime));
            //Wait for a frame
            yield return null;
        }
        Colored = false;
    }
    public static Vector3 averagePosition(MeshRenderer[] targets){
        Vector3 pos = Vector3.zero;
        foreach(var target in targets){
            pos += target.transform.position;
        }
        pos /= targets.Length;
        return pos;
    }
    public void ScaleAround(Transform target, Vector3 pivot){
        Vector3 A = target.transform.localPosition;
        Vector3 B = target.transform.parent.InverseTransformPoint(pivot);

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        // calc final position post-scale
        Vector3 FP = B + C * scaleRatio;



        // finally, actually perform the scale/translation
        target.DOPunchScale(scaleRatio * target.localScale, 0.3f);
    }
}
