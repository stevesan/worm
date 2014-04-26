#pragma strict

public var offsetPixels = Vector2(1, -1);
public var shadowColor = Color.black;

private var shadow:GameObject;
private var shadowText:GUIText;

// Important to do this on Start and NOT Awake..because Awake is called when we instantiate
function Start()
{
    if( shadow == null )
    {
        shadow = new GameObject(gameObject.name+"-textshadow");
        shadow.transform.parent = this.transform.parent;
        shadowText = shadow.AddComponent(GUIText);
    }
}

function LateUpdate()
{
    if( shadow != null )
    {
        shadowText.alignment = guiText.alignment;
        shadowText.anchor = guiText.anchor;
        //shadowText.color = guiText.color;
        shadowText.font = guiText.font;
        shadowText.fontSize = guiText.fontSize;
        shadowText.fontStyle = guiText.fontStyle;
        shadowText.lineSpacing = guiText.lineSpacing;
        shadowText.material = guiText.material;
        shadowText.pixelOffset = guiText.pixelOffset;
        shadowText.richText = guiText.richText;
        shadowText.tabSize = guiText.tabSize;
        shadowText.text = guiText.text;
        shadowText.material.color = shadowColor;

        var myPos = transform.position;
        shadow.transform.position = Vector3(
                myPos.x + offsetPixels.x/Screen.width,
                myPos.y + offsetPixels.y/Screen.height,
                // IMPORTANT: Unity GUI respects the LOCAL Z for some reason....
                transform.localPosition.z - 0.01f
                );
    }
}

// Send this message if you changed it after the LateUpdate
function GUITextPositionChangedLate()
{
    LateUpdate();
}
