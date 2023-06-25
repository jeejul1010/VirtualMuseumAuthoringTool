using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopViewController : MonoBehaviour
{
    public Camera FPVCamera;
    public Camera TopViewCamera;
    public float Speed = 5.0f;
    bool isMove;

    // Start is called before the first frame update
    void Start()
    {
        FPVCamera.enabled = true;
        TopViewCamera.enabled = false;
    }
    public void MoveDisable()
    {
        isMove = false;
    }
    public void MoveEnable()
    {
        isMove = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(TopViewCamera.enabled)
        {
            if(isMove) Move();
        }
        
    }

    // 움직임 실행
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); //Horizontal: A, D 또는 <-, -> 키값이 _moveDirX값에 들어가게됨 (-1, 1, 0)
        float _moveDirZ = Input.GetAxisRaw("Vertical"); //Vertical: 정면과 뒤 W, S 또는 위, 아래 방향키값이 _moveDirY값에 들어가게됨 (-1, 1, 0)

        Vector3 _moveHorizontal = transform.right * _moveDirX; // I창 상의 transform에 _moveDirX를 받아 right으로 움직인다는 뜻
                                                               //vector3는 3D 환경에서의 벡터값 의미; transform.right 은 기본값으로 (1, 0, 0)이 입력되어있음 ; 이를 활용하여 실제 움직임 만듦
        Vector3 _moveVertical = transform.up * _moveDirZ; //transform.forward 은 기본값으로 (0, 0, 1)이 입력되어있음 ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * Speed; //.normalized는 transform.right(1, 0, 0) + transform.forward(0,0,1) 을 더했을 때 2가 되는 것을 (0,5, 0, 0.5)= 1 로 다시 변환해줌 ; Unity가 계산하기 편하게 1로 만들어주는 것을 권장; walkSpeed

        transform.position += _velocity * Time.deltaTime; //deltaTime의 값은 0.016로, update함수를 느리게 만듦
    }

    public void ToggleCamera()
    { 

        if (TopViewCamera.enabled)
        {
            FPVCamera.enabled = true;
            TopViewCamera.enabled = false;
        }
        else
        {
            FPVCamera.enabled = false;
            TopViewCamera.enabled = true;

        }
    }

}
