 #pragma strict
var me : Transform;
var meArrow : Texture2D;
var minimap : Camera;
 
function Start () {
}
 
function Update () {
//}
//function OnGUI()
//{    
    if(minimap.enabled == true)
    {
        var objPos = GetComponent.<Camera>().WorldToViewportPoint(me.transform.position);
        
        var meAngle = me.transform.eulerAngles.y;
        var guiRotationMatrix: Matrix4x4 = GUI.matrix; // set up for GUI rotation
        var pivotMe : Vector2;
        pivotMe.x = Screen.width * (minimap.rect.x + (objPos.x * minimap.rect.width));
        pivotMe.y = Screen.height * (1 - (minimap.rect.y + (objPos.y * minimap.rect.height)));
        GUIUtility.RotateAroundPivot(meAngle, pivotMe);
        
        GUI.DrawTexture(new Rect(Screen.width * (minimap.rect.x + (objPos.x * minimap.rect.width)) - 7.5,
                                    Screen.height * (1 - (minimap.rect.y + (objPos.y * minimap.rect.height))) - 7.5,
                                    15, 15), meArrow);
                                    
        GUI.matrix = guiRotationMatrix; //end GUI rotation                    
    }
}