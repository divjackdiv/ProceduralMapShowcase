using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationStates {  Idle = 0, Walk = 1 }
public class PlayerMapController : MonoBehaviour
{ 
    public Vector2 m_minMaxX;
    public Vector2 m_minMaxY;
    public float m_playerMovementSpeed;
    public Animator m_animator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Walk()
    {
        m_animator.SetFloat("State", (int) AnimationStates.Walk);

    }
}
