using UnityEngine;
using TMPro;

public class PlanetLabel : MonoBehaviour
{
    public TMP_Text nameLabel;
    public float scaleFactor = 1f;
    
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);

        float distance = Vector3.Distance(transform.position, cameraTransform.position);
        float scale = distance * scaleFactor;

        Vector3 parentLossyScale = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
        Vector3 adjustedScale = new Vector3(
            scale / parentLossyScale.x,
            scale / parentLossyScale.y,
            scale / parentLossyScale.z
        );
        
        transform.localScale = adjustedScale;
    }


    public void SetLabel(string name, Color color)
    {
        nameLabel.text = name;
        nameLabel.color = color;
    }
}