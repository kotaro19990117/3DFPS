using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    //変数//

    //移動用
    float x, z;
    //スピード
    float speed = 0.1f;
    //カメラ
    public GameObject cam;
    //回転
    Quaternion cameraRot, characterRot;
    //マウスの感度
    float Xsensitivity = 3f, Ysensitivity = 3f;
    //カーソルtrueで非表示
    bool cursorLock = true;
    //角度の制御
    float minX = -90f, maxX = 90;
    //アニメーション
    public Animator animator;

    

    

    // Start is called before the first frame update
    void Start()
    {
        cameraRot = cam.transform.localRotation;//現在の回転角を入れる
        characterRot = transform.localRotation;//現在の回転角
    }

    // Update is called once per frame
    void Update()
    {
        //マウスの入力を受け取り、その動きをカメラに反映
        float xRot = Input.GetAxis("Mouse X") * Ysensitivity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensitivity;
        cameraRot *= Quaternion.Euler(-yRot, 0, 0);//xにyをいれるよ, +=ではなく*=
        characterRot *= Quaternion.Euler(0, xRot, 0);//yにxをいれるよ, オイラー角だから軸で回す
        cameraRot = ClampRotation(cameraRot);//90~-90に丸める
        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        UpdateCursorLock();//カーソル非表示

        //各ボタンの入力とアニメーション遷移(射撃リロード歩き走り)
        if(Input.GetMouseButton(0))
        {
            animator.SetTrigger("Fire");
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("Reload");
        }

        if(Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)//絶対値
        {
            if(!animator.GetBool("Walk"))//Walkしてないとき
            {
                animator.SetBool("Walk", true);

            }
        }
        else if(animator.GetBool("Walk"))//動いていないのにWalkがTrueのとき
        {
            animator.SetBool("Walk", false);
        }

        if(z > 0 && Input.GetKey(KeyCode.LeftShift))//左のシフトキーかつ前に進むとき
        {
            if(!animator.GetBool("Run"))//Walkしてないとき
            {
                animator.SetBool("Run", true);
                speed = 0.25f;

            }
        }
        else if(animator.GetBool("Run"))//Run判定ではないとき
        {
            animator.SetBool("Run", false);
            speed = 0.1f;
        }
    }

    
    private void FixedUpdate()//Updateはマイフレームで、FixedUpdateは0.02sごと
    {
        //プレイヤーの移動
        x = 0;
        z = 0;

        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        //transform.position += new Vector3(x, 0, z);
        //カメラの正面方向に進むよう. Vector*Scalar
        transform.position += cam.transform.forward * z + cam.transform.right * x;
    }

    //マウスカーソルの表示を切り替える関数
    public void UpdateCursorLock()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if(Input.GetMouseButton(0))//左クリックが押されたとき
        {
            cursorLock = true;
        }

        if(cursorLock)//trueのときbodyに入る
        {
            Cursor.lockState = CursorLockMode.Locked;//非表示
        }
        else if(!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;//非表示
        }
    }

    public Quaternion ClampRotation(Quaternion q)//void=戻り値何もなし, quaternionの何かを返しますという宣言
    {
        //q = x, y, z, w(ベクトルとwはスカラー)
        //q = yRot(cameraRotの-x座標)
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;
        
        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;//Q→Eへ
        angleX = Mathf.Clamp(angleX, minX, maxX);
        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return(q);
    }
}
