using UnityEngine;

public class ComplexObject : MonoBehaviour
{
    public ClickableObject[] components;

    public ClickableObject FindClickableObject(Collider col){
        foreach (var obj in components){
            if (obj.isIn(col)) return obj;
        }
        return null;
    }
    
    public void ColorifyAll(){
        for(int i=0; i<components.Length; i++){
            components[i].Colorify(components[i].components[0].transform.position);
        }
    }

}