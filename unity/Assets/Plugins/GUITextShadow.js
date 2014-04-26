#pragma strict

public var offsetPixels = Vector2(1, -1);
public var shadowColor = Color.black;

private var shadow:GameObject;

// Important to do this on Start and NOT Awake..because Awake is called when we instantiate
function Start()
{
    if( shadow == null )
    {
        shadow = Instantiate(this.gameObject, Vector3.zero, Quaternion.identity);
        shadow.name = this.gameObject.name + "-shadow";
        shadow.transform.parent = this.transform;


        // IMPORTANT: Otherwise it'll recurse infinitely. Destroy it before its Start gets called.
        Destroy(shadow.GetComponent(GUITextShadow));

        shadow.GetComponent(GUIText).material.color = shadowColor;
    }
}

function LateUpdate()
{
    if( shadow != null )
    {
        var shadowText = shadow.GetComponent(GUIText);
        shadowText.text = GetComponent(GUIText).text;
        shadowText.fontSize = guiText.fontSize;

        var gsOffset = Vector3(
                offsetPixels.x/Screen.width,
                offsetPixels.y / Screen.height,
                -0.0001 );
        shadow.transform.localPosition = gsOffset;
    }
}

// Send this message if you changed it after the LateUpdate
function GUITextPositionChangedLate()
{
    LateUpdate();
}
