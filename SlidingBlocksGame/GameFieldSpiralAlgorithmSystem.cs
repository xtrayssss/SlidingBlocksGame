using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpiralGridGenerator : MonoBehaviour
{
    public int x = 0;
    public int y = 0;

    public static IEnumerable<Vector2> GenerateSpiralGridCoordsClockwise()
    {
        var cursorX = 0;
        var cursorY = 0;

        var directionX = 1;
        var directionY = 0;

        var segmentLength = 1;
        var segmentsPassed = 0;

        for (var n = 0; n <= 35; n++)
        {
            // if (!cursorX >= 2 && cursorX < 2 + 2 ||
            //     cursorY >= 2 && cursorY < 2 + 2)
                yield return new Vector2(cursorX, cursorY);

            cursorX += directionX;
            cursorY += directionY;

            segmentsPassed++;
            if (segmentsPassed != segmentLength)
            {
                continue; //if segment is not full yet
            }

            segmentsPassed = 0; //start of new segment

            //rotate clockwise
            var buffer = directionX;
            directionX = -directionY;
            directionY = buffer;

            //increase segmentLength every second time
            if (directionY == 0)
            {
                segmentLength++;
            }
        }
    }

    public async void Start()
    {
        foreach (var vector2 in GenerateSpiralGridCoordsClockwise())
        {
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            primitive.transform.position = new Vector3(vector2.x, 0, vector2.y);

            await Task.Delay(TimeSpan.FromSeconds(0.2f));
        }
    }

    public Vector2 NextPoint()
    {
        return new Vector2(x, y);
    }

    public void Reset()
    {
        x = 0;
        y = 0;
    }
}