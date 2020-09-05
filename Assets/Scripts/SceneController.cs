using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public ImageSynthesis synth;
    public GameObject[] prefabs;
    public int minObjects;
    public int maxObjects;
    public int trainingImages;
    public int validationImages;
    public int frameDivision;
    public bool grayscale;
    public bool save;

    private ShapePool pool;
    private int frameCount = 0;

    void Start()
    {
        pool = ShapePool.Create(prefabs);
    }

    void Update()
    {
        if (frameCount < trainingImages + validationImages){
            if (frameCount % frameDivision == 0){
                GenerateRandom(); 
                Debug.Log($"FrameCount: {frameCount}");
            }
            frameCount++;
            if (save){
                if (frameCount <= trainingImages){
                    //Сохраняем изображения
                    string filename = $"image_{frameCount.ToString().PadLeft(5, '0')}";
                    synth.Save(filename, 512, 512, "captures/train", new int[2] {0, 2});
                }else{
                    int valFrameCount = frameCount - trainingImages;
                    string filename = $"image_{valFrameCount.ToString().PadLeft(5, '0')}";
                    synth.Save(filename, 512, 512, "captures/val", new int[2] {0, 2});
                }
            } 
        }
    }

    void GenerateRandom(){
        pool.ReclaimAll();
        int objectsThisTime = Random.Range(minObjects, maxObjects);
        for (int i = 0; i < objectsThisTime; i++)
        {
            //Выбираем случайный префаб
            int prefabIndx = Random.Range(0, prefabs.Length);
            GameObject prefab = prefabs[prefabIndx];

            //Позиция для префаба
            float newX, newY, newZ;
            newX = Random.Range(-10.0f, 10.0f);
            newY = Random.Range(2.0f, 10.0f);
            newZ = Random.Range(-10.0f, 10.0f);
            
            Vector3 newPos = new Vector3(newX, newY, newZ);
            var newRot = Random.rotation;

            var shape = pool.Get((ShapeLabel)prefabIndx);
            GameObject newObj = shape.obj;
            newObj.transform.position = newPos;
            newObj.transform.rotation = newRot;

            //Размер
            float scaleFactor = Random.Range(0.5f, 2.0f);
            Vector3 newScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            newObj.transform.localScale = newScale;

            //Цвет
            float newR, newG, newB;
            newR = Random.Range(0.0f, 1.0f);
            newG = Random.Range(0.0f, 1.0f);
            newB = Random.Range(0.0f, 1.0f);
            var newColor = new Color(newR, newG, newB);
            newObj.GetComponent<Renderer>().material.color = newColor;
        } 
        synth.OnSceneChange(grayscale);
    }
}
