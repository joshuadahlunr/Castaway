using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace Crew.Globals {
    /// <summary>
    /// Global class for creating and accessing a list of crewmates
    /// </summary>
    /// <author>Misha Desear</author>
    public class Global : MonoBehaviour, IEnumerable<Crew.Crewmates>
    {
        /// <summary>
        /// List of crewmates being managed by this global instance
        /// </summary>
        [SerializeField] protected List<Crew.Crewmates> crewmates = new();
        
        /// <summary>
        /// Access to the number of crewmates stored 
        /// </summary>
        public int Count => crewmates.Count;

        /// <summary>
        /// Array access operator for index based subscripting
        /// </summary>
        /// <param name="i">Index of the crewmate to reference</param>
        public Crew.Crewmates this[int i] 
        {
            get => crewmates[i];
            set => crewmates[i] = value;
        }

        public Crew.Crewmates this[string name]
        {
            get => crewmates.FirstOrDefault(crewmate => crewmate.name == name);
            set 
            {
                var pair = crewmates.WithIndex().FirstOrDefault(crewmate => crewmate.item.name == name);
                if (pair.item is null)
                    throw new ArgumentOutOfRangeException($"The crewmate {name} could not be found in the global list");
                crewmates[pair.index] = value;
            }
        }

        public int Index(Crew.Crewmates crewmate) => crewmates.WithIndex().First(pair => pair.item == crewmate).index;

        public int Index(string name) => crewmates.WithIndex().First(pair => pair.item.name == name).index;

        public IEnumerator<Crew.Crewmates> GetEnumerator() => crewmates.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual void AddCrewMember(Crew.Crewmates crewmate, int index = -1) 
        {
            crewmate.global = this;

            if (index >= 0) crewmates.Insert(index, crewmate);
            else crewmates.Add(crewmate);

            //crewmate.transform.parent = transform;
            //if (index >= 0) crewmate.transform.SetSiblingIndex(index);
        }

        public virtual void RemoveCrewMember(int index) 
        {
            crewmates[index].global = null;
            crewmates.RemoveAt(index);
        }

        public void RemoveCrewMember(string name) => RemoveCrewMember(Index(name));

        public void RemoveCrewMember(Crew.Crewmates crewmate) => RemoveCrewMember(Index(crewmate));

        public void RemoveAllCrewmates() 
        {
            while (Count > 0)
                RemoveCrewMember(0);
        }
    }
}
