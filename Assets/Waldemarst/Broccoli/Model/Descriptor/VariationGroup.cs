using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Broccoli.Model;

namespace Broccoli.Pipe {
    /// <summary>
    /// Variation group class.
    /// </summary>
    [System.Serializable]
    public class VariationGroup {
        #region Vars
        /// <summary>
        /// Characters used to generate a random name for the framing.
        /// </summary>
        const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";
        public int id = 0;
        public string name = "";
        public bool enabled = true;
        public int seed = 0;
        public int minFrequency = 1;
        public int maxFrequency = 4;
        public Vector3 center = Vector3.zero;
        public float radius = 0f;
        public enum OrientationMode {
            CenterToPeriphery,
            PeripheryToCenter,
            clockwise,
            counterClockwise
        }
        public OrientationMode orientation = OrientationMode.CenterToPeriphery;
        public float orientationRandomness = 0f;
        public float minScaleAtCenter = 1f;
        public float maxScaleAtCenter = 1f;
        public float minScaleAtBorder = 1f;
        public float maxScaleAtBorder = 1f;
        public enum BendMode {
            CenterToPeriphery,
            PeripheryToCenter,
            clockwise,
            counterClockwise
        }
        public BendMode bendMode = BendMode.CenterToPeriphery;
        public float minBendAtCenter = 0f;
        public float maxBendAtCenter = 0f;
        public float minBendAtBorder = 0f;
        public float maxBendAtBorder = 0f;
        #endregion

        #region Constructor
        public VariationGroup () {}
        #endregion

        #region Clone
        /// <summary>
        /// Clone this instance.
        /// </summary>
        public VariationGroup Clone () {
            VariationGroup clone = new VariationGroup ();
            clone.id = id;
            clone.name = name;
            clone.enabled = enabled;
            clone.seed = seed;
            clone.minFrequency = minFrequency;
            clone.maxFrequency = maxFrequency;
            clone.center = center;
            clone.radius = radius;
            clone.orientation = orientation;
            clone.orientationRandomness = orientationRandomness;
            clone.minScaleAtCenter = minScaleAtCenter;
            clone.maxScaleAtCenter = maxScaleAtCenter;
            clone.minScaleAtBorder = minScaleAtBorder;
            clone.maxScaleAtBorder = maxScaleAtBorder;
            clone.bendMode = bendMode;
            clone.minBendAtCenter = minBendAtCenter;
            clone.maxBendAtCenter = maxBendAtCenter;
            clone.minBendAtBorder = minBendAtBorder;
            clone.maxBendAtBorder = maxBendAtBorder;
            return clone;
        }
        /// <summary>
		/// Get a random string name.
		/// </summary>
		/// <param name="length">Number of characters.</param>
		/// <returns>Random string name.</returns>
        public static string GetRandomName (int length = 6) {
            string randomName = "";
            for(int i = 0; i < 6; i++) {
                randomName += glyphs [Random.Range (0, glyphs.Length)];
            }
            return randomName;
        }
        #endregion
    }
}