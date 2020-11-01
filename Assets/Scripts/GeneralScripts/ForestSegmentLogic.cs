using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;

namespace Forest
{
    // Component used in the Forest scene, used to randomize how the player moves through the forest.
    public class ForestSegmentLogic : MonoBehaviour
    {
        /// <summary>
        /// Lachlan Pye
        /// Class that contains information for camera bounds, enemy spawns, and the point at which the player spawns from that point.
        /// </summary>
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

            public int id;
            private static int idAssigner = 0;

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

                idAssigner++;
                id = idAssigner;
            }
        }

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

        /// <summary>
        /// Lachlan Pye
        /// If the player is not already moving and this is not the side they started the current segment at, then begin moving the player to the next segment.
        /// </summary>
        public void MoveSegment(string exitSide)
        {
            if (transitioning == false && entrySide != exitSide)
            {
                transitioning = true;
                StartCoroutine(GetNextSegment(exitSide));
                entrySide = sideReverseDict[exitSide];
            }
        }

        /// <summary>
        /// Lachlan Pye
        /// Passes the component an xml node containing information on a new segment. The xml node is parsed for information, and a new ForestSegment instance is created
        /// from the node. It also determines if the segment has an enterance from a particular side, and adds it to the respective ForestSegment list. If the segment
        /// has special tags, those are handled here as well.
        /// </summary>
        /// <param name="segmentNode">The xml node containing the segment information.</param>
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

        /// <summary>
        /// Lachlan Pye
        /// Update the camera bounds using the current segment's information.
        /// </summary>
        public void UpdateBounds()
        {
            worldControl.UpdateBoundsOnly(new Vector3(currentSegment.topLeftCameraBound.x, currentSegment.topLeftCameraBound.y, -10),
                                        new Vector3(currentSegment.bottomRightCameraBound.x, currentSegment.bottomRightCameraBound.y, -10));
        }

        /// <summary>
        /// Lachlan Pye
        /// Get the segment the player is currently in.
        /// </summary>
        /// <returns>The currently active segment.</returns>
        public ForestSegment GetCurrentSegment()
        {
            return currentSegment;
        }

        /// <summary>
        /// Lachlan Pye
        /// Randomly chooses the next segment depending on the side the player exited the last segment from, and what
        /// the tag of the next segment is, and then begins loading that segment. 
        /// </summary>
        /// <param name="exitSide">The side of the segment from which the player is exiting this segment from.</param>
        public IEnumerator GetNextSegment(string exitSide)
        {
            int currentId = currentSegment.id;

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
                        } while (segmentTag != nextSegmentTag && currentId != segmentsWithBottom[rndIndex].id);

                        segment = segmentsWithBottom[rndIndex];
                        break;
                    case "bottom":
                        do
                        {
                            rndIndex = Mathf.RoundToInt(Random.Range(0, segmentsWithTop.Count));
                            segmentTag = segmentsWithTop[rndIndex].tag;
                        } while (segmentTag != nextSegmentTag && currentId != segmentsWithTop[rndIndex].id);

                        segment = segmentsWithTop[rndIndex];
                        break;
                    case "left":
                        do
                        {
                            rndIndex = Mathf.RoundToInt(Random.Range(0, segmentsWithRight.Count));
                            segmentTag = segmentsWithRight[rndIndex].tag;
                        } while (segmentTag != nextSegmentTag && currentId != segmentsWithRight[rndIndex].id);

                        segment = segmentsWithRight[rndIndex];
                        break;
                    case "right":
                        do
                        {
                            rndIndex = Mathf.RoundToInt(Random.Range(0, segmentsWithLeft.Count));
                            segmentTag = segmentsWithLeft[rndIndex].tag;
                        } while (segmentTag != nextSegmentTag && currentId != segmentsWithLeft[rndIndex].id);

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
