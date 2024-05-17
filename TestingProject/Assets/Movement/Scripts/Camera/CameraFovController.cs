using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFovController : MonoBehaviour
{
    private CinemachineVirtualCamera _camera;

    [SerializeField] private float _minFov = 60f;
    [SerializeField] private float _maxFov = 80f;

    [SerializeField] private float _fovSpeed = 1f;

    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
        _camera.m_Lens.FieldOfView = _minFov;
    }

    public void CameraFOV(bool value)
    {
        if (value) _camera.m_Lens.FieldOfView += _fovSpeed * Time.deltaTime;
        else _camera.m_Lens.FieldOfView -= _fovSpeed * Time.deltaTime;

        _camera.m_Lens.FieldOfView = Mathf.Clamp(_camera.m_Lens.FieldOfView, _minFov, _maxFov);
    }
}
