using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPulse : MonoBehaviour
{
    [SerializeField] private float min = 0.5f;
    [SerializeField] private float max = 1.5f;
    [SerializeField] private float speed = 1.0f;

    private Material m_mat;
    private Color m_col;

    void Start()
    {
        m_mat = GetComponentInChildren<Renderer>().material;
        m_col = m_mat.GetColor("_EmissionColor");
    }
	
	void Update()
    {
        m_mat.SetColor("_EmissionColor", m_col * (min + Mathf.PingPong(Time.time * speed, (max - min))));
    }
}
