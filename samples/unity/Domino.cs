using UnityEngine;

public class Domino : MonoBehaviour
{
    private void Start()
    {
        var stage = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stage.name = "Stage";
        stage.transform.localScale = new Vector3(20, 1, 10);

        var startTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        startTile.name = "Start Tile";
        startTile.transform.localScale = new Vector3(0.25F, 2, 1);
        startTile.transform.position = new Vector3(-5, 1.5F, 0);
        startTile.transform.rotation = Quaternion.Euler(0, 0, -10);
        startTile.AddComponent<Rigidbody>();

        for (var i = 0; i < 10; i++)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = $"Tile{i}";
            tile.transform.localScale = new Vector3(0.25F, 2, 1);
            tile.transform.position = new Vector3(-4 + i, 1.5F, 0);
            tile.AddComponent<Rigidbody>();
        }
    }
}
