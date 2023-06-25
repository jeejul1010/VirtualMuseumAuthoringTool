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

    // ������ ����
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); //Horizontal: A, D �Ǵ� <-, -> Ű���� _moveDirX���� ���Ե� (-1, 1, 0)
        float _moveDirZ = Input.GetAxisRaw("Vertical"); //Vertical: ����� �� W, S �Ǵ� ��, �Ʒ� ����Ű���� _moveDirY���� ���Ե� (-1, 1, 0)

        Vector3 _moveHorizontal = transform.right * _moveDirX; // Iâ ���� transform�� _moveDirX�� �޾� right���� �����δٴ� ��
                                                               //vector3�� 3D ȯ�濡���� ���Ͱ� �ǹ�; transform.right �� �⺻������ (1, 0, 0)�� �ԷµǾ����� ; �̸� Ȱ���Ͽ� ���� ������ ����
        Vector3 _moveVertical = transform.up * _moveDirZ; //transform.forward �� �⺻������ (0, 0, 1)�� �ԷµǾ����� ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * Speed; //.normalized�� transform.right(1, 0, 0) + transform.forward(0,0,1) �� ������ �� 2�� �Ǵ� ���� (0,5, 0, 0.5)= 1 �� �ٽ� ��ȯ���� ; Unity�� ����ϱ� ���ϰ� 1�� ������ִ� ���� ����; walkSpeed

        transform.position += _velocity * Time.deltaTime; //deltaTime�� ���� 0.016��, update�Լ��� ������ ����
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
