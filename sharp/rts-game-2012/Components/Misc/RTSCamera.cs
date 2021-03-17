using UnityEngine;

//за основу взят скрипт: http://www.andrejeworutzki.de/game-developement/unity-realtime-strategy-camera/

//по умолчанию камера должна быть иницилизирована в минимальной дальности(т.е. Transform.Position.y=ZoomMin), либо наклоном равным PanAngleMax
public class RTSCamera : MonoBehaviourHeritor
{
    public bool ScrollAreaEnable = false;

    public int ScrollArea = 5;//расстояние до края окна, при котором находящийся там курсор двигает камеру
    public int ScrollSpeed = 50;//скорость перемещения камеры 
    public int DragSpeed = 100;//скорость перемещения камеры, при зажатом колеесике

    public int ZoomSpeed = 20;
    public int ZoomMin = 18;
    public int ZoomMax = 40;

    public int PanSpeed = 80;
    public int PanAngleMin = 25;//мин угол наклона
    public int PanAngleMax = 55;//мax угол наклона



    float _raycastHeigth = 1000f;
    float _currentGroundHeigth;
    float _groundHeigthInStartPosition;//высота камеры в стартовой позиции. Нужна, чтобы высота камеры была на том же уровне, что и в инспекторе до запуска игры

    protected void Start()
    {
        #if UNITY_STANDALONE_WIN
            DragSpeed *= 2;
            PanSpeed *= 2;
            ZoomSpeed *= 2;
        #endif

        //инициализация _currentGroundHeigth
        RaycastHit hit;
        Vector3 raycastPoint = transform.parent.TransformPoint(transform.localPosition);
        raycastPoint.y = _raycastHeigth;
        Ray ray = new Ray(raycastPoint, -Vector3.up);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.GroundLayers))
        {
            _currentGroundHeigth = _raycastHeigth - hit.distance;
            _groundHeigthInStartPosition = _currentGroundHeigth;
            transform.SetY(transform.position.y);
        }
        else
            Debug.LogWarning("Camera position is not over ground");
    }


    void Update()
    {
        var translation = Vector3.zero;
        transform.SetY(transform.position.y - _currentGroundHeigth + _groundHeigthInStartPosition);

        var scrollWheelDelta = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;

        if (scrollWheelDelta != 0 && transform.localEulerAngles.x >= PanAngleMax - 0.1f)// вычитаем 0.1f из-за погрешности перевода в методе localEulerAngles
        {
            translation -= Vector3.up * ZoomSpeed * scrollWheelDelta;
        }

        if (scrollWheelDelta != 0 && transform.localPosition.y <= ZoomMin)
        {
            var pan = transform.eulerAngles.x - scrollWheelDelta * PanSpeed;
            pan = Mathf.Clamp(pan, PanAngleMin, PanAngleMax);
            transform.localEulerAngles = new Vector3(pan, 0, 0);
        }


        // Move camera with arrow keys
        translation += new Vector3(Input.GetAxis("Horizontal") * ScrollSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * ScrollSpeed * Time.deltaTime);

        // Move camera with mouse
        if (Input.GetMouseButton(2)) // MMB
        {
            // Hold button and drag camera around
            translation -= new Vector3(Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime, 0,
                               Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime);
        }
        else
        {
            if (ScrollAreaEnable)
            {
                // Move camera if mouse pointer reaches screen borders
                if (GameManager.CurrentPlayer.CursorState != CursorStates.Selecting)
                {
                    //int mouseScrollSpeed = Mathf.RoundToInt(ScrollSpeed * 0.5f);
                    if (Input.mousePosition.x < ScrollArea)
                        translation += Vector3.right * -ScrollSpeed * Time.deltaTime;

                    if (Input.mousePosition.x >= Screen.width - ScrollArea)
                        translation += Vector3.right * ScrollSpeed * Time.deltaTime;

                    if (Input.mousePosition.y < ScrollArea)
                        translation += Vector3.forward * -ScrollSpeed * Time.deltaTime;

                    if (Input.mousePosition.y > Screen.height - ScrollArea)
                        translation += Vector3.forward * ScrollSpeed * Time.deltaTime;
                }
            }
        }
 
        if (translation!=Vector3.zero)
        {
            Vector3 desiredPosition = transform.localPosition + translation;
            Vector3 newPosition = desiredPosition;

            RaycastHit hit;
            Vector3 raycastPoint = transform.parent.TransformPoint(desiredPosition);
            raycastPoint.y = _raycastHeigth;
            Ray ray = new Ray(raycastPoint, -Vector3.up);

            //если не пересекается с желаемой позицией, то возвращаем предыдущую позицию 
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameManager.GroundLayers))
            {
                _currentGroundHeigth = _raycastHeigth-hit.distance;
            }
            else
            {
                newPosition.x = transform.localPosition.x;
                newPosition.z = transform.localPosition.z;
            }

            //выравнивание позиций
            if (newPosition.y < ZoomMin)
                newPosition.y = ZoomMin;
            else 
                if (newPosition.y > ZoomMax)
                    newPosition.y = ZoomMax;

            transform.localPosition = newPosition;
        }

        transform.SetY(transform.position.y + _currentGroundHeigth - _groundHeigthInStartPosition);
    }
}
