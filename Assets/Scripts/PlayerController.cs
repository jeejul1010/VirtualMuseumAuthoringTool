using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //스피드 조정 변수

    [SerializeField] //private  선언하면 본래 Unity 화면 우측 Inspector창에서 보이지 않게 되는데 (public 선언은 보임), 이때 SeralizeField처리를 하면 보호는 유지하되 I창에서 값을 수정할 수 있게 됨 --> 다시말해 I창에 자동으로 뜨게 만드는 것
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    //걷기
    [SerializeField]
    private float crouchSpeed; //걷기 스피드보다 느림

    private float applySpeed;

    //점프
    [SerializeField]
    private float jumpForce;


    public bool isMove = true;
    public bool isHallLoaded = false;
    //상태 변수
    private bool isCrouch = false;
    private bool isGround = true;

    // 앉을 때 얼마나 앉을 지 결정하는 변수;
    [SerializeField]
    private float crouchPosY; //앉았을 때의 높이
    private float originPosY; //본래의 높이
    private float applyCrouchPosY; //crouchPosY와 orginPosY를 추후 이곳에 대입



    //땅 착지 여부 확인
    private CapsuleCollider capsuleCollider; //Capsule Collider의 바닥 부분이 Mesh 콜라이더와 맞닿아있는 경우를 true, 그렇지 않은 경우를 false

    // 민감도
    [SerializeField]
    private float lookSensitivity; //카메라 민감도

    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0; //정면


    //필요 컴포넌트
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid; //Capsule Collider로 충돌 영역을 설정하고, Rigidbody는 Rigidbody는 Collider에 물리적인 값을 입혀 실체화하는 것 


    // Variable indicating whether the camera is modifed or not
    static public bool isModified = true;

    // Start is called before the first frame update

    void Start() //스크립트 처음 실행시 기본으로 제공되는 것
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        //theCamera = FindObjectOfType<Camera>(); -> 대신 SerializeField를 이용하여 원하는 카메라를 I창에 드래그하여 넣을 수 있도록 함
        myRigid = GetComponent<Rigidbody>(); // GetCompnent - I창 상의 Add Component로 추가된 RigidBody의 Component(값)을 불러오는 함수
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        //cf. transform.position.y;으로 입력할 경우 캐릭터의 본래 y값이 적용되어 플레이어 캡슐 자체가 아래로 이동, 땅에 박히는 현상이 발생
        // localPosition은 World기준이 아닌, 플레이어 위치를 기준
        applyCrouchPosY = originPosY;

        myRigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //Debug.Log("FreezePositionY");

    }

    public void MoveDisable()
    {
        isMove = false;
    }
    public void MoveEnable()
    {
        isMove = true;
    }

    void Update()
    { // Update is called once per frame: 매프레임 마다 호출, 1초에 60회, 따라서 캐릭터 움직임은 이곳에서 구현
        if (isHallLoaded)
        {            
            myRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            IsGround();
            TryJump();
            TryRun(); //반드시 Move(); 함수 위에 있어야 뛰는지 걷는지 판단 후 움직임 가능
            TryCrouch();
            if(isMove) Move();
            if (Input.GetMouseButton(1))
            {
                CameraRotation();
                CharacterRotation();
            }
            //if (!Inventory.inventoryActivated)
            //{
            //    CameraRotation();
            //    CharacterRotation();
            //}
        }

        if (gameObject.transform.position.y < -10)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,  1.0f, gameObject.transform.position.y);
        }
    }

    //앉기 시도

    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //앉기 동작

    private void Crouch()
    {
        isCrouch = !isCrouch; //if (isCrouch) isCrouch = false; else isCrouch = true;의 축약형

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        //theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z); -> 앉기가 딱딱 끊기므로, 지우고 아래의 StartCoroutine 적용
        StartCoroutine(CrouchCoroutine());
    }

    // 부드러운 앉기동작


    IEnumerator CrouchCoroutine() //카메라 이동 부드럽게 처리
                                  // 코딩의 위->아래 흐름으로 처리되는데, 이때 CoRoutine이 삽입될 경우, 해당 함수에 대해 병렬로 동시, 다중처리가 됨 
    {
        float _posY = theCamera.transform.localPosition.y; //임시변수
        int count = 0; // crouch, !crouch시 0에근접하는 값만 나오는 것에 대한 처치
        while (_posY != applyCrouchPosY)
        {
            isModified = true;
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f); //Mathf.Lerp는 (1, 2, 0.5f) 로 1/2씩 증가, 0.5f가 높을수록 빨리 증가
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break; // 15회 crouch 실시 이후 0,1의 근접값이 아닌 정확한 해당 값으로 설정될 수 있도록 처리

            yield return null; //1f 당 CoRoutine을 쉬라는 의미
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f); // break;이후 정확한 값으로 맞춤
        isModified = true;
    }

    // 지면 mesh에 맞닿아있는지 여부 체크
    private void IsGround()
    {
        //myRigid.constraints = RigidbodyConstraints.None;
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); //raycase = 광선쏘는 것; 캡슐이 아닌 고정된 좌표 vector를 기준으로 down으로 한 것
    } //capsuleCollider.bounds.extents.y: 캡슐 콜라이더의 영역(bound)의 반값(extent), 그 중 y의 반값(extents)을 의미 // +0.1f은 대각선/계단면에서의 오차 줄이기 위한 여유

    //점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();

        }

    }

    // 점프 동작
    private void Jump()
    {
        if (isCrouch)
            Crouch();

        myRigid.velocity = transform.up * jumpForce; // transform.up = (0,1,0)
        isModified = true;
    }

    //달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running(); //좌S 키 누르면 달리기 실행
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel(); //좌S 키 떼면 걷기로 다시 전환
        }
    }

    // 달리기 실행
    private void Running()
    {
        if (isCrouch)
            Crouch();

        applySpeed = runSpeed;

    }


    // 달리기 실행
    private void RunningCancel()
    {


        applySpeed = walkSpeed;

    }

    // 움직임 실행
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); //Horizontal: A, D 또는 <-, -> 키값이 _moveDirX값에 들어가게됨 (-1, 1, 0)
        float _moveDirZ = Input.GetAxisRaw("Vertical"); //Vertical: 정면과 뒤 W, S 또는 위, 아래 방향키값이 _moveDirY값에 들어가게됨 (-1, 1, 0)

        Vector3 _moveHorizontal = transform.right * _moveDirX; // I창 상의 transform에 _moveDirX를 받아 right으로 움직인다는 뜻
                                                               //vector3는 3D 환경에서의 벡터값 의미; transform.right 은 기본값으로 (1, 0, 0)이 입력되어있음 ; 이를 활용하여 실제 움직임 만듦
        Vector3 _moveVertical = transform.forward * _moveDirZ; //transform.forward 은 기본값으로 (0, 0, 1)이 입력되어있음 ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; //.normalized는 transform.right(1, 0, 0) + transform.forward(0,0,1) 을 더했을 때 2가 되는 것을 (0,5, 0, 0.5)= 1 로 다시 변환해줌 ; Unity가 계산하기 편하게 1로 만들어주는 것을 권장; walkSpeed

        //myRigid.MovePosition(transform.position + _velocity * Time.deltaTime); //deltaTime의 값은 0.016로, update함수를 느리게 만듦
        transform.position += _velocity * Time.deltaTime; //deltaTime의 값은 0.016로, update함수를 느리게 만듦

        if (_velocity != Vector3.zero)
            isModified = true;
    }

    //상하 카메라 회전
    private void CameraRotation()
    {
        
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); //currntCameraRotationX값이 -45도, +45도 사이에 고정되도록 가두기(clamp)

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); //EulerAngel = rotation X, Y, Z 값이라 보면 됨 

        
        isModified = true;
    }


    //좌우 캐릭터 회전
    private void CharacterRotation()
    {
         
        float _yRotation = Input.GetAxisRaw("Mouse X");
        transform.Rotate(Vector3.up, _yRotation * lookSensitivity, Space.World);
        //Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        //myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // 유니티는 rotation은 vector3가 아니라 Quaternion; 따라서 Euler에서 Quaternion값으로 변환
                                                                                        //Debug.Log(myRigid.rotation);
                                                                                   // Debug.Log(myRigid.rotation.eulerAngles);
        isModified = true;
    }

}
