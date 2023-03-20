using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crew;
using Extensions;
using SQLite;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crew.Globals {
    /// <summary>
    /// Global list of all crew members that exist
    /// </summary>
    /// <author>Misha Desear</author>
    public class CrewList : Global {
        /// <summary>
        /// Type used to store a crewlist in the SQL database
        /// </summary>
        public class GlobalCrewList 
        {
                /// <summary>
                /// The ID of the list; acts as index in CrewListMember table
                /// </summary>
                [PrimaryKey, Unique, AutoIncrement]

                public int id { set; get; }

                /// <summary>
                /// Name of crewlist in database
                /// </summary>
                public string name { set; get; }
        }

        /// <summary>
        /// Type used to store all crew members in a crewlist in the SQL database
        /// </summary>
        public class CrewListMember
        {
                [PrimaryKey, Unique, AutoIncrement]
                public int id { set; get; }

                /// <summary>
                /// The global crewlist that this member is associated with
                /// </summary>
                [NotNull]
                public int listID { set; get; }

                [NotNull]
                public string name { set; get; }
        }

        /// <summary>
        /// List of crewmate prototypes to inject into deck when loaded
        /// </summary>
        public Crewmates[] injectCrewProtos;

        /// <summary>
        /// Crew database of Unity objects that loads are drawn from, allows mapping from SQL names to Unity objects
        /// </summary>
        public CrewDatabase crewDB;

    #if UNITY_EDITOR
        public void Update() {
            if (injectCrewProtos == null) return;

            foreach(var crewProto in injectCrewProtos)
                AddCrewMember(Instantiate(crewProto), 0);

            injectCrewProtos = null;
        }
    #endif

        /// <summary>
        /// Function which can be called to load a crewlist form the SQL database
        /// </summary>  
        /// <param name="name">The name of the crewlist to load</param> 
        /// <param name="clear">If this parameter is true, remove all crew members currently in global list before loading; 
        /// if false, we can additively load crew members to the global list</param> 
        /// <exception cref="ArgumentException"></exception>
        public void DatabaseLoad(string name = "Global Crew", bool clear = true) 
        {
                var crewList = DatabaseManager.GetOrCreateTable<GlobalCrewList>().FirstOrDefault(l => l.name == name);
                if (crewList is null)
                    throw new ArgumentException($"The crewlist {name} could not be found in the database!");

                DatabaseLoad(crewList.id, clear);
        }

        public virtual void DatabaseLoad(int crewlistID, bool clear = true) 
        {
                var crewMembers = DatabaseManager.GetOrCreateTable<CrewListMember>().Where(crewMember => crewMember.listID == crewlistID);

                // If clearing, remove all crewmates currently in global list
                if (clear) RemoveAllCrewmates();

                // If there are any crew members in inject list, add them to front of global list
                foreach (var crewProto in injectCrewProtos) 
                    AddCrewMember(Instantiate(crewProto), 0);
                injectCrewProtos = null;

                // Use associated crewDB to load crew members from the database
                foreach(var crewmates in crewmates) 
                {
                    var instantiated = crewDB.Instantiate(crewmates.name);
                    AddCrewMember(instantiated);
                }
        }
    }
}