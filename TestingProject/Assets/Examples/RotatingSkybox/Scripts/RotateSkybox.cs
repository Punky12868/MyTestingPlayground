using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private Transform target;

    private float yRotation = 0f;

    private void Update()
    {
        TargetRotation();
        Skybox();
    }

    private void TargetRotation() // Weird rotation effect
    {
        float x = Mathf.InverseLerp(-2.5f, 2.5f, target.position.x) * 2 - 1;
        yRotation = Mathf.Lerp(yRotation, -x, Time.deltaTime * lerpSpeed);
    }

    private void Skybox()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
        RenderSettings.skybox.SetVector("_RotationAxis", new Vector4(1f, yRotation, 0f, 0f));
    }
}
