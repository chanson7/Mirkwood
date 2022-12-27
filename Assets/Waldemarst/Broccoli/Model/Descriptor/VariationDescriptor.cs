using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Broccoli.Model;

namespace Broccoli.Pipe {
    /// <summary>
    /// Composite variation container class.
    /// </summary>
    [System.Serializable]
    public class VariationDescriptor {
        #region Composite Variant Unit Descriptor
        [System.Serializable]
        public class VariationUnitDescriptor {
            #region Vars
            public int unitId = 0;
            public int groupId = 0;
            #endregion

            #region Clone
            public VariationUnitDescriptor Clone () {
                VariationUnitDescriptor clone = new VariationUnitDescriptor ();
                clone.unitId = unitId;
                clone.groupId = groupId;
                return clone;
            }
            #endregion
        }
        #endregion

        #region Structure Vars
        public int id = 0;
        public int seed = 0;
        public List<VariationUnitDescriptor> variationUnitDescriptors = new List<VariationUnitDescriptor> ();
        public List<VariationGroup> variationGroups = new List<VariationGroup> ();
        #endregion

        #region Constructor
        public VariationDescriptor () {}
        #endregion

        #region Clone
        /// <summary>
        /// Clone this instance.
        /// </summary>
        public VariationDescriptor Clone () {
            VariationDescriptor clone = new VariationDescriptor ();
            clone.id = id;
            clone.seed = seed;
            for (int i = 0; i < variationUnitDescriptors.Count; i++) {
                clone.variationUnitDescriptors.Add (variationUnitDescriptors [i].Clone ());
            }
            for (int i = 0; i < variationGroups.Count; i++) {
                clone.variationGroups.Add (variationGroups [i].Clone ());
            }
            return clone;
        }
        #endregion

        #region Groups Management
        /// <summary>
        /// Adds a Variation Group to this Variation Descriptor.
        /// </summary>
        /// <param name="groupToAdd"></param>
        public void AddGroup (VariationGroup groupToAdd) {
            // TODO: add id.
            variationGroups.Add (groupToAdd);
        }
        public bool RemoveGroup (int groupIndex) {
            if (groupIndex >= 0 && groupIndex < variationGroups.Count) {
                return true;
            }
            return false;
        }
        #endregion
    }
}