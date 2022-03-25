using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MazeObjects;

public class Compass : MonoBehaviour
{
    private RawImage compass;
    private Character character;
    [SerializeField]
    private Image endPointImage;

    private float compassUnit;
    public Cell endPoint;

    void Start()
    {
        compass = this.GetComponent<RawImage>();
        character = FindObjectOfType<Character>();
        compassUnit = compass.rectTransform.rect.width / 360f;
    }

    // Update is called once per frame
    void Update()
    {
        compass.uvRect = new Rect(character.transform.localEulerAngles.y / 360f, 0f, 1f, 1f);

        endPointImage.rectTransform.anchoredPosition = GetPosOnCompass();
    }

    private Vector2 GetPosOnCompass()
    {
        Vector2 characterPos = new Vector2(character.transform.position.x, character.transform.position.z);
        Vector2 characterFwd = new Vector2(character.transform.forward.x, character.transform.forward.z);
        Vector2 endPointPos = new Vector2(endPoint.obj.transform.position.x, endPoint.obj.transform.position.z);
        float angle = Vector2.SignedAngle(endPointPos - characterPos, characterFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
