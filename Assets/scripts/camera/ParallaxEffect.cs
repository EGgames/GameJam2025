using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxEffect : MonoBehaviour
{
    [Tooltip("Cantidad de parallax que se aplica al fondo (1 = misma velocidad que la cámara). [0, 1]")]
    public float amountOfParallax = 1;
    [Tooltip("Cámara principal a seguir.")]
    public Camera mainCamera;
    [Tooltip("Si el fondo se repite (tileset).")]
    public bool isRepeating;

    private float _startingPosX;
    private float _startingPosY;
    private float _lengthX;
    private float _lengthY;

    void Start()
    {
        _startingPosX = transform.position.x;
        _startingPosY = transform.position.y;
        _lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        _lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        float tempX = mainCamera.transform.position.x * (1 - amountOfParallax);
        float distanceX = mainCamera.transform.position.x * amountOfParallax;
        float tempY = mainCamera.transform.position.y * (1 - amountOfParallax);
        float distanceY = mainCamera.transform.position.y * amountOfParallax;

        transform.position = new Vector3(_startingPosX + distanceX, _startingPosY + distanceY, transform.position.z);

        if (!isRepeating) return;

        if (tempX > _startingPosX + _lengthX)
        {
            _startingPosX += _lengthX;
        }
        else if (tempX < _startingPosX - _lengthX)
        {
            _startingPosX -= _lengthX;
        }

        if (tempY > _startingPosY + _lengthY)
        {
            _startingPosY += _lengthY;
        }
        else if (tempY < _startingPosY - _lengthY)
        {
            _startingPosY -= _lengthY;
        }
    }
}