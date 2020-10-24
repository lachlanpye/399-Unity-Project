using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;

namespace Forest
{
    public class ForestSegmentLogic : MonoBehaviour
    {
        public class ForestSegment
        {
            public Vector2 topLeftCameraBound;
            public Vector2 bottomRightCameraBound;

            public List<Vector2> enemySpawns;

            public Vector2 topInPoint;
            public Vector2 bottomInPoint;
            public Vector2 leftInPoint;
            public Vector2 rightInPoint;

            public string tag;

            public void Constructor(Vector2 topLeftCameraBound, Vector2 bottomRightCameraBound, Vector2 topInPoint, Vector2 bottomInPoint, Vector2 leftInPoint, Vector2 rightInPoint, List<Vector2> enemySpawns, string tag)
            {
                this.topLeftCameraBound = topLeftCameraBound;
                this.bottomRightCameraBound = bottomRightCameraBound;

                this.enemySpawns = enemySpawns;

                this.topInPoint = topInPoint;
                this.bottomInPoint = bottomInPoint;
                this.leftInPoint = leftInPoint;
                this.rightInPoint = rightInPoint;

                this.tag = tag;
            }
        }

        public float cameraWidthFromCenter;
        public float cameraHeightFromCenter;

        [Space]
        [Tooltip("Possible tags: initial, peace, combat, exit")]
        public string[] levelStructure;
        [Space]
        public GameObject enemyPrefab;
        public GameObject enemyParentObject;

        private List<ForestSegment> segmentsWithTop;
        private List<ForestSegment> segmentsWithBottom;
        private List<ForestSegment> segmentsWithLeft;
        private List<ForestSegment> segmentsWithRight;
        private ForestSegment exitSegment;

        private ForestSegment currentSegment;
        private WorldControl worldControl;
        private PlayerBehaviour playerBehaviour;

        private string entrySide;
        private bool transitioning;
        private int levelStructureIndex;

        private Dictionary<string, string> sideReverseDict;

        private bool completedSetup = false;

        void Update()
        {
            Debug.DrawLine(worldControl.mainCamera.transform.position, new Vector3(worldControl.mainCamera.transform.position.x + cameraWidthFromCenter, worldControl.mainCamera.transform.position.y, worldControl.mainCamera.transform.position.z), Color.green);
            Debug.DrawLine(worldControl.mainCamera.transform.position, new Vector3(worldControl.mainCamera.transform.position.x, worldControl.mainCamera.transform.position.y + cameraHeightFromCenter, worldControl.mainCamera.transform.position.z), Color.green);

            if (currentSegment != null && transitioning == false)
            {
                if (playerBehaviour.gameObject.transform.position.y - playerBehaviour.distanceDownFromPlayerCenter >= currentSegment.topLeftCameraBound.y + cameraHeightFromCenter
                    && entrySide != "top")
                {
                    MoveSegment("top");
                }
                else if (playerBehaviour.gameObject.transform.position.y - playerBehaviour.distanceDownFromPlayerCenter <= currentSegment.bottomRightCameraBound.y - cameraHeightFromCenter
                    && entrySide != "bottom")
                {
                    MoveSegment("bottom");
                }
                else if (playerBehaviour.gameObject.transform.position.x <= currentSegment.topLeftCameraBound.x - cameraWidthFromCenter
                    && entrySide != "left")
                {
                    MoveSegment("left");
                }
                else if (playerBehaviour.gameObject.transform.position.x >= currentSegment.bottomRightCameraBound.x + cameraWidthFromCenter
                    && entrySide != "right")
                {
                    MoveSegment("right");
                }
            }
        }

        public void MoveSegment(string exitSide)
        {
            if (transitioning == false && entrySide != exitSide)
            {
                transitioning = true;
                StartCoroutine(GetNextSegment(exitSide));
                entrySide = sideReverseDict[exitSide];
            }
        }

        public void PopulateArrays(XmlNode segmentNode)
        {
            if (completedSetup == false)
            {
                currentSegment = null;
                transitioning = false;

                segmentsWithTop = new List<ForestSegment>();
                segmentsWithBottom = new List<ForestSegment>();
                segmentsWithLeft = new List<ForestSegment>();
                segmentsWithRight = new List<ForestSegment>();
                exitSegment = null;

                worldControl = GetComponent<WorldControl>();
                playerBehaviour = worldControl.playerObject.GetComponent<PlayerBehaviour>();

                sideReverseDict = new Dictionary<string, string>()
                {
                    { "top", "bottom" },
                    { "bottom", "top" },
                    { "left", "right" },
                    { "right", "left" }
                };

                entrySide = "";
                levelStructureIndex = 0;
                completedSetup = true;
            }

            ForestSegment newSegment = new ForestSegment();

            List<Vector2> enemySpawns = new List<Vector2>();
            float topLeftCameraBoundX = 0, topLeftCameraBoundY = 0, bottomRightCameraBoundX = 0, bottomRightCameraBoundY = 0, topEntryX = 0, topEntryY = 0, bottomEntryX = 0, bottomEntryY = 0, leftEntryX = 0, leftEntryY = 0, rightEntryX = 0, rightEntryY = 0;
            string tag = "";
            for (int i = 0; i < segmentNode.ChildNodes.Count; i++)
            {
                tag = segmentNode.Attributes["tag"].Value;
                XmlNode childNode = segmentNode.ChildNodes[i];
                switch (childNode.Name)
                {
                    case "topLeftBoundX":
                        topLeftCameraBoundX = float.Parse(childNode.InnerText);
                        break;

                    case "topLeftBoundY":
                        topLeftCameraBoundY = float.Parse(childNode.InnerText);
                        break;

                    case "bottomRightBoundX":
                        bottomRightCameraBoundX = float.Parse(childNode.InnerText);
                        break;

                    case "bottomRightBoundY":
                        bottomRightCameraBoundY = float.Parse(childNode.InnerText);
                        break;

                    case "enemy":
                        enemySpawns.Add(new Vector2(float.Parse(childNode.Attributes["x"].Value), float.Parse(childNode.Attributes["y"].Value)));
                        break;

                    case "top":
                        topEntryX = float.Parse(childNode.Attributes["entryPointX"].Value);
                        topEntryY = float.Parse(childNode.Attributes["entryPointY"].Value);

                        segmentsWithTop.Add(newSegment);
                        break;

                    case "bottom":
                        bottomEntryX = float.Parse(childNode.Attributes["entryPointX"].Value);
                        bottomEntryY = float.Parse(childNode.Attributes["entryPointY"].Value);

                        segmentsWithBottom.Add(newSegment);
                        break;

                    case "left":
                        leftEntryX = float.Parse(childNode.Attributes["entryPointX"].Value);
                        leftEntryY = float.Parse(childNode.Attributes["entryPointY"].Value);

                        segmentsWithLeft.Add(newSegment);
                        break;

                    case "right":
                        rightEntryX = float.Parse(childNode.Attributes["entryPointX"].Value);
                        rightEntryY = float.Parse(childNode.Attributes["entryPointY"].Value);

                        segmentsWithRight.Add(newSegment);
                        break;

                    default:
                        break;
                }
            }
            newSegment.Constructor(new Vector2(topLeftCameraBoundX, topLeftCameraBoundY),
                                    new Vector2(bottomRightCameraBoundX, bottomRightCameraBoundY),
                                    new Vector2(topEntryX, topEntryY),
                                    new Vector2(bottomEntryX, bottomEntryY),
                                    new Vector2(leftEntryX, leftEntryY),
                                    new Vector2(rightEntryX, rightEntryY),
                                    new List<Vector2>(enemySpawns), tag);

            if (tag == "initial")
            {
                currentSegment = newSegment;
                UpdateBounds();
            }

            if (tag == "exit")
            {
                exitSegment = newSegment;
            }
        }
        public void SegmentCounts()
        {
            Debug.Log("top: " + segmentsWithTop.Count);
            Debug.Log("bottom: " + segmentsWithBottom.Count);
            Debug.Log("left: " + segmentsWithLeft.Count);
            Debug.Log("right: " + segmentsWithRight.Count);
        }
        public void UpdateBounds()
        {
            worldControl.UpdateBounds2(new Vector3(currentSegment.topLeftCameraBound.x, currentSegment.topLeftCameraBound.y, -10),
                                        new Vector3(currentSegment.bottomRightCameraBound.x, currentSegment.bottomRightCameraBound.y, -10));
        }

        public ForestSegment GetCurrentSegment()
        {
            return currentSegment;
        }
        public IEnumerator GetNextSegment(string exitSide)
        {
            ForestSegment segment = new ForestSegment();
            levelStructureIndex++;
            string nextSegmentTag = levelStructure[levelStructureIndex];

            if (nextSegmentTag == "exit")
            {
                segment = exitSegment;
            }
            else
            {
                int rndIndex;
                string segmentTag = "";
                switch (exitSide)
                {
                    case "top":
                        do
                        {
                            rndIndex = Mathf.RoundToInt(Random.Range(0, segmentsWithBottom.Count));
                            segmentTag = segmentsWithBottom[rndIndex].tag;
                        } while (segmentTag != nextSegmentTag);

                        segment = segmentsWithBottom[rndIndex];
                        break;
                    case "bottom":
                        do
                        {
                            rndIndex = Mathf.RoundToInt(Random.Range(0, segmentsWithTop.Count));
                            segmentTag = segmentsWithTop[rndIndex].tag;
                        } while (segmentTag != nextSegmentTag);

                        segment = segmentsWithTop[rndIndex];
                        break;
                    case "left":
                        do
                        {
                            rndIndex = Mathf.RoundToInt(Random.Range(0, segmentsWithRight.Count));
                            segmentTag = segmentsWithRight[rndIndex].tag;
                        } while (segmentTag != nextSegmentTag);

                        segment = segmentsWithRight[rndIndex];
                        break;
                    case "right":
                        do
                        {
                            rndIndex = Mathf.RoundToInt(Random.Range(0, segmentsWithLeft.Count));
                            segmentTag = segmentsWithLeft[rndIndex].tag;
                        } while (segmentTag != nextSegmentTag);

                        segment = segmentsWithLeft[rndIndex];
                        break;

                    default:
                        break;
                }
            }

            currentSegment = segment;
            yield return StartCoroutine(worldControl.CoroutineMoveSegments(segment, enemyPrefab, enemyParentObject, exitSide));
            transitioning = false;
        }
    }
}
